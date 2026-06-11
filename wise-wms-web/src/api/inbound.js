import request from "./request";

export function getInboundOrders(params) {
  return request.get("/api/InboundOrders", { params });
}

export function getInboundOrder(id) {
  return request.get(`/api/InboundOrders/${id}`);
}

export function createInboundOrder(data) {
  return request.post("/api/InboundOrders", data);
}
