import request from "./request";

export function getProducts(params) {
  return request.get("/api/Products", { params });
}

export function getProductById(id) {
  return request.get(`/api/Products/${id}`);
}

export function createProduct(data) {
  return request.post("/api/Products", data);
}

export function updateProduct(id, data) {
  return request.put(`/api/Products/${id}`, data);
}

export function deleteProduct(id) {
  return request.delete(`/api/Products/${id}`);
}
