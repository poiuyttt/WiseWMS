import request from "./request";

export function getTransactions(params) {
  return request.get("/api/Inventory/transactions", { params });
}

export function getLowStockProducts() {
  return request.get("/api/Inventory/low-stock");
}
