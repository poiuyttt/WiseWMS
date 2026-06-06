# .NET 8 进销存管理系统 — 项目计划

## 技术栈

| 层 | 技术 |
|--|--|
| 后端 | ASP.NET Core 8 Web API |
| 前端 | Vue 3 + Vue Router + Pinia + Element Plus + Axios |
| ORM | Entity Framework Core / SqlSugar |
| 数据库 | SQL Server |
| 认证 | JWT (BCrypt 加密) |
| 校验 | FluentValidation |
| 映射 | AutoMapper |

---

## 项目结构

### 后端 (一个解决方案三个项目)

```
InventorySystem.sln
├── InventorySystem.Api               # WebAPI 项目
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ProductsController.cs
│   │   ├── InboundOrdersController.cs
│   │   ├── OutboundOrdersController.cs
│   │   ├── InventoryController.cs
│   │   └── DashboardController.cs
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs     # 全局异常处理
│   └── Program.cs
│
├── InventorySystem.Application       # Service / DTO / 业务逻辑
│   ├── DTOs/
│   │   ├── LoginDto.cs / LoginResultDto.cs
│   │   ├── ProductDto.cs
│   │   ├── InboundOrderDto.cs
│   │   ├── OutboundOrderDto.cs
│   │   ├── InventoryDto.cs
│   │   └── PagedResult.cs            # 通用分页模型
│   ├── Services/
│   │   ├── IAuthService.cs / AuthService.cs
│   │   ├── IProductService.cs / ProductService.cs
│   │   ├── IInboundService.cs / InboundService.cs
│   │   ├── IOutboundService.cs / OutboundService.cs
│   │   ├── IInventoryService.cs / InventoryService.cs
│   │   └── IDashboardService.cs / DashboardService.cs
│   ├── Validators/                   # FluentValidation 校验器
│   └── Profiles/                     # AutoMapper Profile
│
└── InventorySystem.Infrastructure    # EFCore + Repository
    ├── Data/
    │   ├── AppDbContext.cs
    │   └── SeedData.cs               # 种子数据（默认管理员账号）
    ├── Entities/
    │   ├── User.cs
    │   ├── Category.cs
    │   ├── Supplier.cs
    │   ├── Customer.cs
    │   ├── Product.cs
    │   ├── InboundOrder.cs
    │   ├── InboundItem.cs
    │   ├── OutboundOrder.cs
    │   ├── OutboundItem.cs
    │   └── InventoryTransaction.cs
    ├── Repositories/
    └── Migrations/
```

### 前端

```
inventory-web/
├── src/
│   ├── api/                    # Axios 封装 + 每个模块的 API 调用
│   │   ├── request.ts          # Axios 实例 + 拦截器
│   │   ├── auth.ts
│   │   ├── product.ts
│   │   ├── inbound.ts
│   │   ├── outbound.ts
│   │   ├── inventory.ts
│   │   └── dashboard.ts
│   ├── router/
│   │   └── index.ts            # 路由 + 守卫
│   ├── stores/
│   │   └── user.ts             # Pinia 用户/Token 状态
│   ├── layouts/
│   │   └── MainLayout.vue      # 侧边栏 + 顶栏布局
│   ├── views/
│   │   ├── login/index.vue
│   │   ├── dashboard/index.vue
│   │   ├── product/index.vue     # 商品列表
│   │   ├── product/Form.vue      # 新增/编辑商品
│   │   ├── inbound/index.vue     # 入库单列表
│   │   ├── inbound/Form.vue      # 新建入库单
│   │   ├── inbound/Detail.vue    # 入库单详情
│   │   ├── outbound/index.vue    # 出库单列表
│   │   ├── outbound/Form.vue     # 新建出库单
│   │   ├── outbound/Detail.vue   # 出库单详情
│   │   └── inventory/index.vue   # 库存查询 + 流水
│   └── App.vue
```

---

## 数据库设计（10 张表）

### 1. Users (用户)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int PK | |
| Username | nvarchar(50) | 登录名 |
| PasswordHash | nvarchar(200) | BCrypt 哈希 |
| DisplayName | nvarchar(50) | 显示名 |
| Role | nvarchar(20) | Admin / Operator |
| CreatedAt | datetime2 | |

### 2. Categories (商品分类)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int PK | |
| Name | nvarchar(100) | 分类名称 |
| ParentId | int? | 父级分类（支持多级） |

### 3. Suppliers (供应商)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int PK | |
| Name | nvarchar(100) | |
| Contact | nvarchar(50) | 联系人 |
| Phone | nvarchar(20) | |
| Address | nvarchar(200) | |

### 4. Customers (客户)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int PK | |
| Name | nvarchar(100) | |
| Contact | nvarchar(50) | |
| Phone | nvarchar(20) | |
| Address | nvarchar(200) | |

### 5. Products (商品)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int PK | |
| Name | nvarchar(100) | 商品名称 |
| Spec | nvarchar(100) | 规格 |
| Unit | nvarchar(10) | 单位（个/箱/kg） |
| CategoryId | int FK | 关联分类 |
| Price | decimal(18,2) | 售价 |
| Stock | int | 当前库存量 |
| MinStock | int | 最低库存预警线 |
| Description | nvarchar(500) | 备注 |
| CreatedAt | datetime2 | |

### 6. InboundOrders (入库单)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int PK | |
| OrderNo | nvarchar(50) | 单号（自动生成 IN20260606001） |
| SupplierId | int FK | 供应商 |
| OperatorId | int FK | 操作人 |
| TotalAmount | decimal(18,2) | 入库总金额 |
| Remark | nvarchar(500) | |
| CreatedAt | datetime2 | |

### 7. InboundItems (入库明细)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int PK | |
| InboundOrderId | int FK | |
| ProductId | int FK | |
| Quantity | int | 数量 |
| UnitPrice | decimal(18,2) | 入库单价 |

### 8. OutboundOrders (出库单)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int PK | |
| OrderNo | nvarchar(50) | 单号（自动生成 OUT20260606001） |
| CustomerId | int FK | 客户 |
| OperatorId | int FK | 操作人 |
| TotalAmount | decimal(18,2) | 出库总金额 |
| Remark | nvarchar(500) | |
| CreatedAt | datetime2 | |

### 9. OutboundItems (出库明细)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int PK | |
| OutboundOrderId | int FK | |
| ProductId | int FK | |
| Quantity | int | 数量 |
| SalePrice | decimal(18,2) | 售价 |

### 10. InventoryTransactions (库存流水 — 审计日志)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | |
| ProductId | int FK | 关联商品 |
| Type | nvarchar(10) | In / Out / Adjustment |
| Quantity | int | 变动数量（正=入库，负=出库） |
| StockBefore | int | 变动前库存 |
| StockAfter | int | 变动后库存 |
| OrderNo | nvarchar(50) | 关联单号 |
| Remark | nvarchar(200) | |
| CreatedAt | datetime2 | |
| OperatorId | int FK | 操作人 |

---

## 核心业务逻辑说明

### 入库流程
1. 前端选择供应商 + 商品列表（商品 + 数量 + 单价）
2. 后端校验商品是否存在
3. 开启数据库事务：
   - 写入 InboundOrders + InboundItems
   - **使用 `XLock` 行级锁锁定 Product 行，防止并发**
   - 更新 Products.Stock += Quantity
   - 写入 InventoryTransactions 流水
4. 提交事务

### 出库流程
1. 前端选择客户 + 商品列表（商品 + 数量）
2. 后端校验：
   - 商品是否存在
   - **Stock >= Quantity（库存充足）**
3. 开启事务：
   - 写入 OutboundOrders + OutboundItems
   - `XLock` 锁定 Product 行
   - 更新 Products.Stock -= Quantity
   - 写入 InventoryTransactions 流水
4. 提交事务（库存不足则回滚）

### 并发安全
- 使用 `RepeatableRead` 事务隔离级别 + `WITH (UPDLOCK)` 行锁
- 伪代码：`await _context.Products.FromSqlRaw("SELECT * FROM Products WITH (UPDLOCK, ROWLOCK) WHERE Id = {0}", id).FirstOrDefaultAsync()`

---

## 表格对照（修正前 → 修正后）

| 修正前 | 修正后 | 原因 |
|--------|--------|------|
| 5 张表 | 10 张表 | 缺少分类/供应商/客户/流水 |
| OutboundItems 无售价 | 增加 SalePrice | 算不出来销售额 |
| 直接改 Stock | 加 InventoryTransactions | 审计追踪 |
| 无并发控制 | 行锁 | 防超卖 |
| 无校验 | FluentValidation | 简历加分 |
| 无异常处理 | ExceptionMiddleware | 统一错误返回 |
| 无映射 | AutoMapper | 代码整洁 |

---

## 开发顺序（推荐按这个节奏）

### 第一阶段：基础搭建（1-2 天）
1. 创建后端解决方案 + 三个项目结构
2. AppDbContext + 实体类 + 迁移
3. 种子数据（管理员账号 + 示例分类/商品）
4. JWT 认证配置 + AuthController
5. 异常中间件
6. Vue 3 项目初始化 + 路由 + 布局

### 第二阶段：核心 CRUD（4-5 天）
7. 商品管理（后端 + 前端页面）
8. 入库单（后端 + 前端页面）
9. 出库单（后端 + 前端页面）
10. 库存查询 + 流水

### 第三阶段：增值功能（2 天）
11. Dashboard 图表
12. Excel 导出
13. 库存预警

### 第四阶段：收尾（1 天）
14. Docker Compose 部署配置
15. 写 README.md
16. 提交 GitHub

---

## 面试常问 & 需要在项目里体现的点

| 问题 | 你在项目里的回答 |
|------|----------------|
| 如何防止库存超卖？ | RepeatableRead + 行锁，扣减前校验 |
| 为什么用 AutoMapper？ | 隔离实体和 DTO，避免循环引用，简化代码 |
| JWT 过期怎么处理？ | Access Token 短过期 + Refresh Token |
| 如何保证数据一致性？ | 事务 + 出入库都写流水日志 |
| 分页查询怎么优化的？ | 先 Count 再 Skip/Take，加索引 |
| 用户密码怎么存的？ | BCrypt 哈希，不存明文 |
