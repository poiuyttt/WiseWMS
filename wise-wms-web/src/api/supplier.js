import request from "./request";

export function getSuppliers(params) {
  return request.get("/api/Suppliers", { params });
}

export function getSupplierById(id) {
  return request.get(`/api/Suppliers/${id}`);
}

export function createSupplier(data) {
  return request.post("/api/Suppliers", data);
}

export function updateSupplier(id, data) {
  return request.put(`/api/Suppliers/${id}`, data);
}

export function deleteSupplier(id) {
  return request.delete(`/api/Suppliers/${id}`);
}
