<script setup>
import { ref, reactive, onMounted, watch } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import {
  getProducts,
  deleteProduct,
  createProduct,
  updateProduct,
} from "@/api/product";

const keyword = ref("");
const products = ref([]);
const total = ref(0);
const page = ref(1);
const pageSize = ref(10);
const role = localStorage.getItem("role");

const showDialog = ref(false);
const editId = ref(0);
const form = reactive({
  name: "",
  spec: "",
  unit: "",
  categoryId: null,
  price: 0,
  minStock: 0,
  description: "",
});

async function del(id) {
  await ElMessageBox.confirm("确认删除该商品？");
  await deleteProduct(id);
  ElMessage.success("删除成功");
  load();
}

function edit(row) {
  editId.value = row.id;
  Object.assign(form, row);
  showDialog.value = true;
}

function resetForm() {
  editId.value = 0;
  form.name = "";
  form.spec = "";
  form.unit = "";
  form.categoryId = null;
  form.price = 0;
  form.minStock = 0;
  form.description = "";
  showDialog.value = false;
}

async function save() {
  if (editId.value) {
    await updateProduct(editId.value, form);
    ElMessage.success("修改成功");
  } else {
    await createProduct(form);
    ElMessage.success("新增成功");
  }
  resetForm();
  load();
}

async function load() {
  const res = await getProducts({
    keyword: keyword.value,
    page: page.value,
    pageSize: pageSize.value,
  });
  products.value = res.items;
  total.value = res.total;
}

onMounted(load);

watch([keyword, page], load);
</script>
<template>
  <div>
    <el-row :gutter="20">
      <el-col :span="12">
        <el-input
          v-model="keyword"
          placeholder="搜索商品名称/规格"
          clearable
        ></el-input>
      </el-col>
      <el-col :span="12" style="text-align: right">
        <el-button
          type="primary"
          @click="
            editId = 0;
            form.name = '';
            showDialog = true;
          "
          >新增商品</el-button
        >
      </el-col>
    </el-row>

    <el-table :data="products" stripe style="margin-top: 20px">
      <el-table-column prop="id" label="ID" width="60" />
      <el-table-column prop="name" label="名称" />
      <el-table-column prop="spec" label="规格" />
      <el-table-column prop="unit" label="单位" width="60" />
      <el-table-column prop="categoryName" label="分类" />
      <el-table-column prop="price" label="价格" width="100" />
      <el-table-column prop="stock" label="库存" width="80" />
      <el-table-column label="操作" width="150">
        <template #default="{ row }">
          <el-button size="small" @click="edit(row)">编辑</el-button>
          <el-button
            v-if="role === 'Admin'"
            size="small"
            type="danger"
            @click="del(row.id)"
            >删除</el-button
          >
        </template>
      </el-table-column>
    </el-table>

    <el-dialog
      v-model="showDialog"
      :title="editId ? '编辑商品' : '新增商品'"
      width="500px"
    >
      <el-form :model="form" label-width="80px">
        <el-form-item label="商品名">
          <el-input v-model="form.name" />
        </el-form-item>
        <el-form-item label="规格">
          <el-input v-model="form.spec" />
        </el-form-item>
        <el-form-item label="单位">
          <el-input v-model="form.unit" />
        </el-form-item>
        <el-form-item label="分类">
          <el-select v-model="form.categoryId" placeholder="请选择">
            <el-option label="食品饮料" :value="1" />
            <el-option label="日用品" :value="2" />
            <el-option label="电子数码" :value="3" />
            <el-option label="办公用品" :value="4" />
          </el-select>
        </el-form-item>
        <el-form-item label="售价">
          <el-input-number v-model="form.price" :min="0" />
        </el-form-item>
        <el-form-item label="预警库存">
          <el-input-number v-model="form.minStock" :min="0" />
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="form.description" type="textarea" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showDialog = false">取消</el-button>
        <el-button type="primary" @click="save">保存</el-button>
      </template>
    </el-dialog>

    <div style="display: flex; justify-content: center; margin-top: 20px">
      <el-pagination
        background
        layout="total,prev,pager,next"
        :total="total"
        :page-size="pageSize"
        v-model:current-page="page"
      />
    </div>
  </div>
</template>
