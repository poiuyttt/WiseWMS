<script setup>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { getDashboard } from "@/api/dashboard";

const router = useRouter();
const data = ref({});

onMounted(async () => {
  data.value = await getDashboard();
});
</script>
<template>
  <div>
    <el-row :gutter="20">
      <el-col :span="6">
        <el-card shadow="hover" style="text-align: center">
          <h2 style="color: #409eff">{{ data.totalProducts ?? "-" }}</h2>
          <p>商品总数</p>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" style="text-align: center">
          <h2 style="color: #67c23a">{{ data.totalStock ?? "-" }}</h2>
          <p>库存总量</p>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" style="text-align: center">
          <h2 style="color: #e6a23c">{{ data.todayInbound ?? "-" }}</h2>
          <p>今日入库</p>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" style="text-align: center">
          <h2 style="color: #909399">{{ data.todayOutbound ?? "-" }}</h2>
          <p>今日出库</p>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="20" style="margin-top: 20px">
      <el-col :span="6">
        <el-card
          shadow="hover"
          style="text-align: center; cursor: pointer"
          @click="router.push('/inventory')"
        >
          <h2 style="color: #f56c6c">{{ data.lowStockCount ?? "-" }}</h2>
          <p>
            低库存预警
            <span style="font-size: 12px; color: #999">点击查看</span>
          </p>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>
