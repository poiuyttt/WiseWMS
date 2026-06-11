<script setup>
import { ref, computed, onMounted, watch } from "vue";
import { ElMessage } from "element-plus";
import {
  getOutboundOrders,
  getOutboundOrder,
  createOutboundOrder,
} from "@/api/outbound";
import { getProducts } from "@/api/product";
import request from "@/api/request";

const keyword = ref("");
const orders = ref([]);
const total = ref(0);
const page = ref(1);
const pageSize = ref(10);

async function load() {
  const res = await getOutboundOrders({
    keyword: keyword.value,
    page: page.value,
    pageSize: pageSize.value,
  });
  orders.value = res.items;
  total.value = res.total;
}
onMounted(load);
watch([keyword, page], load);

const customers = ref([]);
const products = ref([]);
const showCreate = ref(false);
const submitting = ref(false);

const createForm = ref({ customerId: null, remark: "", items: [] });
const createTotal = computed(() => {
  return createForm.value.items.reduce(
    (sum, it) => sum + (it.salePrice || 0) * it.quantity,
    0,
  );
});

function addItem() {
  createForm.value.items.push({ productId: null, quantity: 1, salePrice: 0 });
}

async function openCreate() {
  const [cRes, pRes] = await Promise.all([
    request.get("/api/Customers"),
    getProducts({ page: 1, pageSize: 9999 }),
  ]);
  customers.value = cRes;
  products.value = pRes.items;
  createForm.value = {
    customerId: null,
    remark: "",
    items: [{ productId: null, quantity: 1, salePrice: 0 }],
  };
  showCreate.value = true;
}

function resetCreate() {
  showCreate.value = false;
}

async function submitCreate() {
  if (!createForm.value.customerId) return ElMessage.warning("请选择客户");
  if (!createForm.value.items.length)
    return ElMessage.warning("请添加至少一个商品");
  submitting.value = true;
  try {
    await createOutboundOrder(createForm.value);
    ElMessage.success("出库单创建成功");
    showCreate.value = false;
    load();
  } finally {
    submitting.value = false;
  }
}

const showDetail = ref(false);
const detail = ref({});
async function viewDetail(row) {
  const res = await getOutboundOrder(row.id);
  detail.value = res;
  showDetail.value = true;
}
</script>
<template>
  <div>
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
        <el-button type="primary" @click="openCreate">新建出库单</el-button>
      </el-col>
    </el-row>

    <el-table :data="orders" stripe border>
      <el-table-column prop="orderNo" label="单号" width="180" />
      <el-table-column prop="customerName" label="客户" />
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

    <div style="display: flex; justify-content: center; margin-top: 20px">
      <el-pagination
        background
        layout="total,prev,pager,next"
        :total="total"
        :page-size="pageSize"
        v-model:current-page="page"
      />
    </div>

    <el-dialog
      v-model="showCreate"
      title="新建出库单"
      width="700px"
      @close="resetCreate"
    >
      <el-form :model="createForm" label-width="80">
        <el-form-item label="客户">
          <el-select
            v-model="createForm.customerId"
            placeholder="选择客户"
            style="width: 100%"
          >
            <el-option
              v-for="c in customers"
              :key="c.id"
              :label="c.name"
              :value="c.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="createForm.remark" type="textarea" />
        </el-form-item>
      </el-form>

      <el-divider>出库明细</el-divider>

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
        <el-table-column label="售价" width="120">
          <template #default="{ $index }">
            <el-input-number
              v-model="createForm.items[$index].salePrice"
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
          >提交出库</el-button
        >
      </template>
    </el-dialog>

    <el-dialog v-model="showDetail" title="出库单详情" width="700px">
      <el-descriptions :column="2" border>
        <el-descriptions-item label="单号">{{
          detail.orderNo
        }}</el-descriptions-item>
        <el-descriptions-item label="客户">{{
          detail.customerName
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
        <el-table-column prop="salePrice" label="售价">
          <template #default="{ row }"
            >¥{{ row.salePrice.toFixed(2) }}</template
          >
        </el-table-column>
        <el-table-column label="小计">
          <template #default="{ row }"
            >¥{{ (row.quantity * row.salePrice).toFixed(2) }}</template
          >
        </el-table-column>
      </el-table>
    </el-dialog>
  </div>
</template>
