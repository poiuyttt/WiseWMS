import { defineStore } from "pinia";

export const useUserStore = defineStore("user", {
  state: () => ({
    token: localStorage.getItem("token") || "",
    displayName: localStorage.getItem("displayName") || "",
    role: localStorage.getItem("role") || "",
  }),
  actions: {
    login(res) {
      this.token = res.token;
      this.displayName = res.displayName;
      this.role = res.role;
      localStorage.setItem("token", res.token);
      localStorage.setItem("displayName", res.displayName);
      localStorage.setItem("role", res.role);
    },
    logout() {
      this.token = "";
      this.displayName = "";
      this.role = "";
      localStorage.clear();
    },
  },
});
