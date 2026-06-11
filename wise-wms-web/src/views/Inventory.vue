<script setup>
import { ref, onMounted, watch } from "vue";
import { getProducts } from "@/api/product";
import { getTransactions } from "@/api/inventory";

const keyword = ref("");
const products = ref([]);
const total = ref(0);
const page = ref(1);
const pageSize = ref(10);

/* 库存流水对话框 */
const showTx = ref(false);
const currentProductId = ref(0);
const txProductName = ref("");
const txs = ref([]);
const txTotal = ref(0);
const txPage = ref(1);
const txPageSize = ref(10);

async function load() {
  const res = await getProducts({
    keyword: keyword.value,
    page: page.value,
    pageSize: pageSize.value,
  });
  products.value = res.items;
  total.value = res.total;
}

async function viewTransactions(row) {
  currentProductId.value = row.id;
  txProductName.value = row.name;
  txPage.value = 1;
  await loadTx();
  showTx.value = true;
}

async function loadTx() {
  if (!currentProductId.value) return;
  const res = await getTransactions({
    productId: currentProductId.value,
    page: txPage.value,
    pageSize: txPageSize.value,
  });
  txs.value = res.items;
  txTotal.value = res.total;
}

watch(txPage, loadTx);

function stockStatus(row) {
  if (row.stock <= 0) return { text: "缺货", type: "danger" };
  if (row.stock <= row.minStock) return { text: "预警", type: "warning" };
  return { text: "正常", type: "success" };
}

onMounted(load);
watch([keyword, page], load);
</script>
<template>
  <div>
    <el-row :gutter="20">
      <el-col :span="12">
        <el-input v-model="keyword" placeholder="搜索商品名称/规格" clearable />
      </el-col>
    </el-row>

    <el-table :data="products" stripe style="margin-top: 20px">
      <el-table-column prop="id" label="ID" width="60" />
      <el-table-column prop="name" label="名称" />
      <el-table-column prop="spec" label="规格" />
      <el-table-column prop="unit" label="单位" width="60" />
      <el-table-column prop="categoryName" label="分类" />
      <el-table-column prop="stock" label="当前库存" width="100" />
      <el-table-column prop="minStock" label="预警线" width="80" />
      <el-table-column label="状态" width="80">
        <template #default="{ row }">
          <el-tag :type="stockStatus(row).type">{{
            stockStatus(row).text
          }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="100">
        <template #default="{ row }">
          <el-button size="small" @click="viewTransactions(row)"
            >流水</el-button
          >
        </template>
      </el-table-column>
    </el-table>

    <div style="display: flex; justify-content: center; margin-top: 20px">
      <el-pagination
        background
        layout="total,prev,pager,next"
        :total="total"
        :page-size="pageSize"
        v-model:current-page="page"
      />
    </div>

    <!-- 库存流水对话框 -->
    <el-dialog
      v-model="showTx"
      :title="`库存流水 - ${txProductName}`"
      width="800px"
    >
      <el-table :data="txs" border stripe>
        <el-table-column prop="type" label="类型" width="80">
          <template #default="{ row }">
            <el-tag :type="row.type === 'In' ? 'success' : 'danger'">{{
              row.type === "In" ? "入库" : "出库"
            }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="quantity" label="变动数量" width="100">
          <template #default="{ row }">
            <span :style="{ color: row.quantity > 0 ? '#67c23a' : '#f56c6c' }"
              >{{ row.quantity > 0 ? "+" : "" }}{{ row.quantity }}</span
            >
          </template>
        </el-table-column>
        <el-table-column prop="stockBefore" label="变动前" width="80" />
        <el-table-column prop="stockAfter" label="变动后" width="80" />
        <el-table-column prop="orderNo" label="关联单号" width="180" />
        <el-table-column prop="operatorName" label="操作人" />
        <el-table-column prop="createdAt" label="时间" width="180">
          <template #default="{ row }">{{
            row.createdAt.slice(0, 16).replace("T", " ")
          }}</template>
        </el-table-column>
      </el-table>
      <div style="display: flex; justify-content: center; margin-top: 20px">
        <el-pagination
          background
          layout="total,prev,pager,next"
          :total="txTotal"
          :page-size="txPageSize"
          v-model:current-page="txPage"
        />
      </div>
    </el-dialog>
  </div>
</template>
