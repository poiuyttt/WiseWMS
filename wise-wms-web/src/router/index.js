import { createRouter, createWebHashHistory } from "vue-router";

const routes = [
  {
    path: "/",
    component: () => import("@/layouts/MainLayout.vue"),
    children: [
      { path: "", redirect: "/dashboard" },
      { path: "dashboard", component: () => import("@/views/Dashboard.vue") },
      { path: "products", component: () => import("@/views/Products.vue") },
      { path: "inbound", component: () => import("@/views/InboundOrders.vue") },
      {
        path: "outbound",
        component: () => import("@/views/OutboundOrders.vue"),
      },
      { path: "inventory", component: () => import("@/views/Inventory.vue") },
    ],
  },
  { path: "/login", component: () => import("@/views/Login.vue") },
];

const router = createRouter({
  history: createWebHashHistory(),
  routes,
});

//路由守卫
router.beforeEach((to) => {
  const token = localStorage.getItem("token");
  if (to.path !== "/login" && !token) {
    return "/login";
  }
});

export default router;
