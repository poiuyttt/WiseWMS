<script setup>
import { ref, reactive } from "vue";
import { useRouter } from "vue-router";
import { ElMessage } from "element-plus";
import { login } from "@/api/auth";

const router = useRouter();
const loading = ref(false);

const form = reactive({
  username: "",
  password: "",
});

async function handleLogin() {
  if (!form.username || !form.password) {
    ElMessage.error("请输入用户名和密码");
    return;
  }
  loading.value = true;
  try {
    const res = await login(form);
    localStorage.setItem("token", res.token);
    localStorage.setItem("displayName", res.displayName);
    localStorage.setItem("role", res.role);
    ElMessage.success("登录成功");
    router.push("/");
  } catch {
  } finally {
    loading.value = false;
  }
}
</script>
<template>
  <div class="login-container">
    <el-card class="login-card">
      <h2>WiseWMS 登录</h2>
      <el-form :model="form" label-width="80px">
        <el-form-item label="用户名">
          <el-input
            v-model="form.username"
            placeholder="请输入用户名"
          ></el-input>
        </el-form-item>
        <el-form-item label="密码">
          <el-input v-model="form.password" placeholder="请输入密码"></el-input>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleLogin" :loading="loading"
            >登录</el-button
          >
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>
<style scoped>
.login-container {
  height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: #f0f2f5;
}
.login-card {
  width: 400px;
}
</style>
