<script setup>
import { ref, onMounted } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { getCategories, createCategory, deleteCategory } from "@/api/category";
import { useUserStore } from "@/stores/user";

const categories = ref([]);
const dialogVisible = ref(false);
const newName = ref("");
const role = computed(() => useUserStore().role);

async function load() {
  categories.value = await getCategories();
}

async function handleCreate() {
  if (!newName.value.trim()) {
    ElMessage.warning("请输入分类名称");
    return;
  }
  try {
    await createCategory({ name: newName.value.trim() });
    ElMessage.success("新增成功");
    dialogVisible.value = false;
    newName.value = "";
    load();
  } catch {
    ElMessage.error("新增失败，名称可能已存在");
  }
}

async function handleDelete(id, name) {
  if (role !== "Admin") {
    ElMessage.warning("仅管理员可删除");
    return;
  }
  try {
    await ElMessageBox.confirm(`确认删除分类「${name}」？`);
    await deleteCategory(id);
    ElMessage.success("删除成功");
    load();
  } catch (err) {
    if (err !== "cancel") ElMessage.error("删除失败，分类下可能有商品");
  }
}

onMounted(load);
</script>

<template>
  <div>
    <el-row :gutter="20">
      <el-col :span="12">
        <h2>商品分类</h2>
      </el-col>
      <el-col :span="12" style="text-align: right">
        <el-button
          v-if="role === 'Admin'"
          type="primary"
          @click="dialogVisible = true"
        >
          新增分类
        </el-button>
      </el-col>
    </el-row>

    <el-table :data="categories" stripe style="margin-top: 20px">
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="name" label="分类名称" />
      <el-table-column label="操作" width="120">
        <template #default="{ row }">
          <el-button
            v-if="role === 'Admin'"
            size="small"
            type="danger"
            @click="handleDelete(row.id, row.name)"
          >
            删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog v-model="dialogVisible" title="新增分类" width="400px">
      <el-input
        v-model="newName"
        placeholder="请输入分类名称"
        @keyup.enter="handleCreate"
      />
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleCreate">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>
