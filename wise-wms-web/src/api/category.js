import request from "./request";

export function getCategories() {
  return request.get("/api/Categories");
}

export function createCategory(data) {
  return request.post("/api/Categories", data);
}

export function deleteCategory(id) {
  return request.delete(`/api/Categories/${id}`);
}
