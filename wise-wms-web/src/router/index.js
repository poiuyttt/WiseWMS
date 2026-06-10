import { createRouter, createWebHashHistory } from "vue-router";

const routes = [
  { path: "/", redirect: "/dashboard" },
  {
    path: "/login",
    component: () => import("@/views/Login.vue"),
  },
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
