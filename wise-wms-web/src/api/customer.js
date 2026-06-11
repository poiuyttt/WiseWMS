import request from "./request";

export function getCustomers(params) {
  return request.get("/api/Customers", { params });
}

export function getCustomerById(id) {
  return request.get(`/api/Customers/${id}`);
}

export function createCustomer(data) {
  return request.post("/api/Customers", data);
}

export function updateCustomer(id, data) {
  return request.put(`/api/Customers/${id}`, data);
}

export function deleteCustomer(id) {
  return request.delete(`/api/Customers/${id}`);
}
