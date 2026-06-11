<script setup>
import { useRoute, useRouter } from "vue-router";
import { computed } from "vue";

const route = useRoute();
const router = useRouter();
const displayName = computed(() => localStorage.getItem("displayName") || "");
const role = computed(() => localStorage.getItem("role") || "");

function logout() {
  localStorage.clear();
  router.push("/login");
}
</script>
<template>
  <el-container class="layout">
    <el-aside width="220px"
      ><div
        style="
          padding: 20px;
          border-bottom: 1px solid #dcdfe6;
          text-align: center;
        "
      >
        <strong>{{ displayName }}</strong>
        <el-tag size="small" :type="role === 'Admin' ? 'danger' : 'info'">{{
          role
        }}</el-tag>
      </div>
      <el-menu :router="true" :default-active="route.path">
        <el-menu-item index="/dashboard">📊 Dashboard</el-menu-item>
        <el-menu-item index="/products">📦 商品管理</el-menu-item>
        <el-menu-item index="/inbound">📥 入库单</el-menu-item>
        <el-menu-item index="/outbound">📤 出库单</el-menu-item>
        <el-menu-item index="/suppliers">🏭 供应商管理</el-menu-item>
        <el-menu-item index="/customers">👥 客户管理</el-menu-item>
        <el-menu-item index="/inventory">🔍 库存查询</el-menu-item>
        <el-menu-item index="/login" @click="logout">🚪 退出</el-menu-item>
      </el-menu>
    </el-aside>
    <el-main>
      <router-view />
    </el-main>
  </el-container>
</template>
<style scoped>
.layout {
  height: 100%;
  overflow: auto;
}
.el-main {
  background: #f5f7fa;
}
</style>
