<script setup>
import { useRoute, useRouter } from "vue-router";

const route = useRoute();
const router = useRouter();

function logout() {
  localStorage.clear();
  router.push("/login");
}
</script>
<template>
  <el-container class="layout">
    <el-aside width="220px">
      <el-menu
        :router="true"
        :default-active="route.path"
        style="height: 100vh"
      >
        <el-menu-item index="/dashboard">📊 Dashboard</el-menu-item>
        <el-menu-item index="/products">📦 商品管理</el-menu-item>
        <el-menu-item index="/inbound">📥 入库单</el-menu-item>
        <el-menu-item index="/outbound">📤 出库单</el-menu-item>
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
  height: 100vh;
}
.el-main {
  background: #f5f7fa;
}
</style>
