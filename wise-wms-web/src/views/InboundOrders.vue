<script setup>
import { ref, computed, onMounted, watch } from "vue";
import { ElMessage } from "element-plus";
import {
  getInboundOrders,
  getInboundOrder,
  createInboundOrder,
} from "@/api/inbound";
import { getProducts } from "@/api/product";
import request from "@/api/request";

const keyword = ref("");
const orders = ref([]);
const total = ref(0);
const page = ref(1);
const pageSize = ref(10);

async function load() {
  const res = await getInboundOrders({
    keyword: keyword.value,
    page: page.value,
    pageSize: pageSize.value,
  });
  orders.value = res.items;
  total.value = res.total;
}
onMounted(load);
watch([keyword, page], load);

const suppliers = ref([]);
const products = ref([]);
const showCreate = ref(false);
const submitting = ref(false);

const createForm = ref({ supplierId: null, remark: "", items: [] });
const createTotal = computed(() => {
  return createForm.value.items.reduce(
    (sum, it) => sum + (it.unitPrice || 0) * it.quantity,
    0,
  );
});

function addItem() {
  createForm.value.items.push({
    productId: null,
    quantity: 1,
    unitPrice: 0,
  });
}

async function openCreate() {
  const [sRes, pRes] = await Promise.all([
    request.get("/api/Suppliers"),
    getProducts({ page: 1, pageSize: 9999 }),
  ]);
  suppliers.value = sRes;
  products.value = pRes.items;
  createForm.value = {
    supplierId: null,
    remark: "",
    items: [{ productId: null, quantity: 1, unitPrice: 0 }],
  };
  showCreate.value = true;
}

function resetCreate() {
  showCreate.value = false;
}

async function submitCreate() {
  if (!createForm.value.items.length)
    return ElMessage.warning("请添加至少一个商品");
  try {
    await formRef.value.validate();
  } catch {
    return;
  }
  submitting.value = true;
  try {
    await createInboundOrder(createForm.value);
    ElMessage.success("入库单创建成功");
    showCreate.value = false;
    load();
  } finally {
    submitting.value = false;
  }
}

const showDetail = ref(false);
const detail = ref({});

async function viewDetail(row) {
  const res = await getInboundOrder(row.id);
  detail.value = res;
  showDetail.value = true;
}

const formRef = ref(null);

const rules = {
  supplierId: [{ required: true, message: "请选择供应商", trigger: "change" }],
};
</script>
<template>
  <div>
    <!-- 搜索栏 -->
    <el-row :gutter="16" style="margin-bottom: 20px">
      <el-col :span="8">
        <el-input
          v-model="keyword"
          placeholder="搜索单号"
          clearable
          @clear="load"
        />
      </el-col>
      <el-col :span="16" style="text-align: right">
        <el-button type="primary" @click="openCreate">新建入库单</el-button>
      </el-col>
    </el-row>

    <!-- 入库单列表 -->
    <el-table :data="orders" stripe border>
      <el-table-column prop="orderNo" label="单号" width="180" />
      <el-table-column prop="supplierName" label="供应商" />
      <el-table-column prop="operatorName" label="操作人" />
      <el-table-column prop="totalAmount" label="总金额" width="120">
        <template #default="{ row }"
          >¥{{ row.totalAmount.toFixed(2) }}</template
        >
      </el-table-column>
      <el-table-column prop="createdAt" label="时间" width="180">
        <template #default="{ row }">{{
          row.createdAt.slice(0, 16).replace("T", " ")
        }}</template>
      </el-table-column>
      <el-table-column label="操作" width="120">
        <template #default="{ row }">
          <el-button size="small" @click="viewDetail(row)">详情</el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 分页 -->
    <div style="display: flex; justify-content: center; margin-top: 20px">
      <el-pagination
        background
        layout="total,prev,pager,next"
        :total="total"
        :page-size="pageSize"
        v-model:current-page="page"
      />
    </div>

    <!-- 创建入库单对话框 -->
    <el-dialog
      v-model="showCreate"
      title="新建入库单"
      width="700px"
      @close="resetCreate"
    >
      <el-form
        ref="formRef"
        :model="createForm"
        :rules="rules"
        label-width="80px"
      >
        <el-form-item label="供应商" prop="supplierId">
          <el-select
            v-model="createForm.supplierId"
            placeholder="选择供应商"
            style="width: 100%"
          >
            <el-option
              v-for="s in suppliers"
              :key="s.id"
              :label="s.name"
              :value="s.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="createForm.remark" type="textarea" />
        </el-form-item>
      </el-form>

      <el-divider>入库明细</el-divider>

      <el-button
        type="success"
        size="small"
        @click="addItem"
        style="margin-bottom: 10px"
        >+ 添加商品</el-button
      >

      <el-table :data="createForm.items" border stripe>
        <el-table-column label="商品" width="220">
          <template #default="{ $index }">
            <el-select
              v-model="createForm.items[$index].productId"
              placeholder="选择商品"
              filterable
              style="width: 100%"
            >
              <el-option
                v-for="p in products"
                :key="p.id"
                :label="`${p.name} (${p.spec})`"
                :value="p.id"
              />
            </el-select>
          </template>
        </el-table-column>
        <el-table-column label="数量" width="120">
          <template #default="{ $index }">
            <el-input-number
              v-model="createForm.items[$index].quantity"
              :min="1"
              style="width: 100%"
            />
          </template>
        </el-table-column>
        <el-table-column label="单价" width="120">
          <template #default="{ $index }">
            <el-input-number
              v-model="createForm.items[$index].unitPrice"
              :min="0.01"
              :precision="2"
              style="width: 100%"
            />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="80">
          <template #default="{ $index }">
            <el-button
              type="danger"
              size="small"
              @click="createForm.items.splice($index, 1)"
              >删除</el-button
            >
          </template>
        </el-table-column>
      </el-table>

      <template #footer>
        <span v-if="createTotal">合计: ¥{{ createTotal.toFixed(2) }}</span>
        <el-button type="primary" @click="submitCreate" :loading="submitting"
          >提交入库</el-button
        >
      </template>
    </el-dialog>

    <!-- 详情对话框 -->
    <el-dialog v-model="showDetail" title="入库单详情" width="700px">
      <el-descriptions :column="2" border>
        <el-descriptions-item label="单号">{{
          detail.orderNo
        }}</el-descriptions-item>
        <el-descriptions-item label="供应商">{{
          detail.supplierName
        }}</el-descriptions-item>
        <el-descriptions-item label="操作人">{{
          detail.operatorName
        }}</el-descriptions-item>
        <el-descriptions-item label="时间">{{
          detail.createdAt?.slice(0, 16).replace("T", " ")
        }}</el-descriptions-item>
        <el-descriptions-item label="备注" :span="2">{{
          detail.remark
        }}</el-descriptions-item>
      </el-descriptions>
      <el-divider>明细</el-divider>
      <el-table :data="detail.items" border stripe>
        <el-table-column prop="productName" label="商品" />
        <el-table-column prop="productSpec" label="规格" />
        <el-table-column prop="quantity" label="数量" />
        <el-table-column prop="unitPrice" label="单价">
          <template #default="{ row }"
            >¥{{ row.unitPrice.toFixed(2) }}</template
          >
        </el-table-column>
        <el-table-column label="小计">
          <template #default="{ row }"
            >¥{{ (row.quantity * row.unitPrice).toFixed(2) }}</template
          >
        </el-table-column>
      </el-table>
    </el-dialog>
  </div>
</template>
