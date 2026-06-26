import { createRouter, createWebHashHistory } from "vue-router";
import { useUserStore } from "@/stores/user";

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
      { path: "suppliers", component: () => import("@/views/Suppliers.vue") },
      { path: "customers", component: () => import("@/views/Customers.vue") },
      { path: "categories", component: () => import("@/views/Categories.vue") },
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
  const userStore = useUserStore();
  const token = userStore.token;
  if (to.path !== "/login" && !token) {
    return "/login";
  }
});

export default router;
