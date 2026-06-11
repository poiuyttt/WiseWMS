import request from "./request";

export function getOutboundOrders(params) {
  return request.get("/api/OutboundOrder", { params });
}

export function getOutboundOrder(id) {
  return request.get(`/api/OutboundOrder/${id}`);
}

export function createOutboundOrder(data) {
  return request.post("/api/OutboundOrder", data);
}
