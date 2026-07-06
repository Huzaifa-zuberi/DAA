import streamlit as st
import networkx as nx
import folium
import joblib
import requests
from datetime import datetime
from streamlit_folium import st_folium

from graph import LANDMARKS, ROADS, build_graph, heuristic

# ── Page config ──────────────────────────────────────────
st.set_page_config(
    page_title="Karachi Traffic AI",
    page_icon="🚦",
    layout="wide"
)

# ── Load model ───────────────────────────────────────────
@st.cache_resource
def load_model():
    return joblib.load("traffic_model.pkl")

model = load_model()

# ── UI ───────────────────────────────────────────────────
st.title("🚦 Karachi Traffic Prediction using AI + A*")
st.caption("Real-time route optimization using Random Forest + A* Search Algorithm")

with st.sidebar:
    st.header("🗺️ Route Settings")
    origin = st.selectbox("Origin", list(LANDMARKS.keys()))
    destination = st.selectbox("Destination", list(LANDMARKS.keys()), index=1)
    st.divider()
    st.info("Select origin and destination, then click **Find Best Route**.")

# ── Traffic prediction ───────────────────────────────────
def predict_traffic():
    now = datetime.now()

    if 7 <= now.hour <= 9 or 17 <= now.hour <= 19:
        avg_hourly = 0.10       # Peak hours
    elif 0 <= now.hour <= 5:
        avg_hourly = 0.005      # Late night
    else:
        avg_hourly = 0.055      # Normal hours

    sample = [[now.hour, now.day, now.month, avg_hourly]]
    return model.predict(sample)[0]

# ── OSRM road geometry ───────────────────────────────────
def get_road_coords(start_name, end_name):
    """Get actual road geometry from OSRM (free, no API key needed)"""
    lat1, lon1 = LANDMARKS[start_name]
    lat2, lon2 = LANDMARKS[end_name]

    url = (
        f"http://router.project-osrm.org/route/v1/driving/"
        f"{lon1},{lat1};{lon2},{lat2}"
        f"?overview=full&geometries=geojson"
    )

    try:
        response = requests.get(url, timeout=5)
        data = response.json()
        coords = data["routes"][0]["geometry"]["coordinates"]
        # OSRM returns [lon, lat] — flip to [lat, lon] for Folium
        return [[lat, lon] for lon, lat in coords]
    except Exception:
        # Fallback to straight line if OSRM is unreachable
        return [[lat1, lon1], [lat2, lon2]]

# ── Map builder ──────────────────────────────────────────
def build_map(path, G):
    m = folium.Map(
        location=[24.8607, 67.0104],
        zoom_start=12,
        tiles="CartoDB positron"
    )

    # Draw all roads in grey (straight lines for background context)
    for a, b in ROADS:
        lat1, lon1 = LANDMARKS[a]
        lat2, lon2 = LANDMARKS[b]
        folium.PolyLine(
            [[lat1, lon1], [lat2, lon2]],
            color="#cccccc",
            weight=2,
            opacity=0.5
        ).add_to(m)

    # Draw best route following actual Karachi roads via OSRM
    for i in range(len(path) - 1):
        road_coords = get_road_coords(path[i], path[i + 1])
        folium.PolyLine(
            road_coords,
            color="#00C853",
            weight=6,
            opacity=0.9,
            tooltip=f"{path[i]} → {path[i + 1]}"
        ).add_to(m)

    # Draw markers for all landmarks
    for name, (lat, lon) in LANDMARKS.items():
        if name == path[0]:
            icon = folium.Icon(color="green", icon="play", prefix="fa")
        elif name == path[-1]:
            icon = folium.Icon(color="red", icon="flag", prefix="fa")
        elif name in path:
            icon = folium.Icon(color="blue", icon="circle", prefix="fa")
        else:
            icon = folium.Icon(color="gray", icon="circle", prefix="fa")

        folium.Marker(
            [lat, lon],
            tooltip=name,
            popup=name,
            icon=icon
        ).add_to(m)

    return m

# ── Main action ──────────────────────────────────────────
if st.button("🔍 Find Best Route", use_container_width=True):

    if origin == destination:
        st.warning("⚠️ Origin and destination cannot be the same.")
    else:
        with st.spinner("Calculating optimal route..."):
            traffic_factor = predict_traffic()
            G = build_graph(traffic_factor)

            try:
                path = nx.astar_path(
                    G,
                    origin,
                    destination,
                    heuristic=heuristic,
                    weight="weight"
                )

                # Calculate estimated travel time
                total_weight = sum(
                    G[path[i]][path[i + 1]]["weight"]
                    for i in range(len(path) - 1)
                )
                avg_speed_kmh = 30
                travel_time_min = (total_weight / avg_speed_kmh) * 60

                # ── Results ──
                st.success("✅ Route Found Successfully!")

                col1, col2, col3 = st.columns(3)
                col1.metric("🚗 Predicted Traffic", f"{round(traffic_factor)} vehicles/hr")
                col2.metric("📍 Stops", len(path))
                col3.metric("⏱️ Est. Time", f"{round(travel_time_min)} min")

                st.write("### 🗺️ Route")
                st.write(" ➜ ".join(path))

                st.write("### 📍 Map")
                route_map = build_map(path, G)

                st_folium(
                    route_map,
                    use_container_width=True,
                    height=550,
                    returned_objects=[]
                )

            except nx.NetworkXNoPath:
                st.error(f"❌ No route found between {origin} and {destination}.")
            except nx.NodeNotFound:
                st.error("❌ Selected location not found in the graph.")