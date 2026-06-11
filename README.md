# WiseWMS 进销存管理系统

基于 **.NET 8 + Vue 3** 的企业级进销存管理系统，支持商品管理、入库/出库管理、库存查询与流水追踪。

---

## 技术栈

### 后端
- **框架**：ASP.NET Core 8 Web API
- **架构**：三层架构（API / Application / Infrastructure）
- **ORM**：Entity Framework Core 8
- **数据库**：SQL Server 2022
- **认证**：JWT Bearer + BCrypt 密码哈希
- **API 文档**：Swagger / OpenAPI

### 前端
- **框架**：Vue 3（Composition API / `<script setup>`）
- **UI 组件库**：Element Plus
- **路由**：Vue Router 5
- **状态管理**：Pinia
- **HTTP 客户端**：Axios
- **构建工具**：Vite 8

### 部署
- **容器化**：Docker Compose（三容器编排：SQL Server / API / Web）

---

## 功能模块

| 模块 | 功能说明 |
|---|---|
| 🔐 登录认证 | JWT Token 登录，Token 自动注入请求头，过期自动跳转登录页 |
| 🏠 仪表盘 | 今日/累计入库金额、今日/累计出库金额、商品总数、库存总量、近期入库/出库趋势 |
| 📦 商品管理 | 商品 CRUD、按名称规格搜索、分页、关联分类 |
| 🏢 供应商管理 | 供应商 CRUD、搜索、分页 |
| 👥 客户管理 | 客户 CRUD、搜索、分页 |
| 📥 入库管理 | 新建入库单（选供应商 + 多商品明细）、列表查询、详情查看、自动更新库存 |
| 📤 出库管理 | 新建出库单（选客户 + 多商品明细）、列表查询、详情查看、自动扣减库存 |
| 📊 库存管理 | 实时库存查询、按商品搜索、库存流水记录（入库/出库自动产生流水） |

---

## 项目结构

```
WiseWMS/
├── WiseWMS.Api/                    # 后端解决方案
│   ├── WiseWMS.Api/                #   API 层
│   │   ├── Controllers/            #     REST API 控制器
│   │   ├── Middleware/             #     全局异常中间件
│   │   ├── Program.cs              #     启动配置 + 依赖注入 + 种子数据
│   │   └── appsettings.json        #     数据库连接 + JWT 配置
│   ├── WiseWMS.Application/        #   业务逻辑层
│   │   ├── DTOs/                   #     数据传输对象
│   │   ├── Services/               #     业务服务实现
│   │   └── Services/Interfaces/    #     服务接口
│   ├── WiseWMS.Infrastructure/     #   数据访问层
│   │   ├── Data/                   #     AppDbContext + 种子数据
│   │   ├── Entities/               #     数据库实体
│   │   └── Migrations/             #     EF Core 迁移
│   └── Dockerfile                  #   后端容器化
├── wise-wms-web/                   # 前端项目
│   ├── src/
│   │   ├── api/                    #   API 请求封装（axios 拦截器 + 各模块请求）
│   │   ├── views/                  #   页面组件（Dashboard / Products / ...）
│   │   ├── layouts/                #   布局组件
│   │   ├── router/                 #   路由配置
│   │   ├── main.js                 #   入口
│   │   └── App.vue                 #   根组件
│   └── Dockerfile                  #   前端容器化（Nginx 托管）
├── docker-compose.yml              # 一键启动编排配置
└── README.md                       # 本文件
```

### 后端分层说明

| 项目 | 职责 |
|---|---|
| `WiseWMS.Api` | HTTP 接口层：接收请求、认证授权、返回响应；不包含业务逻辑 |
| `WiseWMS.Application` | 业务逻辑层：Service 类实现业务规则、DTO 映射、事务控制 |
| `WiseWMS.Infrastructure` | 数据访问层：EF Core DbContext、实体定义、数据库迁移 |

---

## 快速启动

### 方式一：Docker Compose（推荐，一键启动全部）

```bash
# 在项目根目录执行
docker-compose up -d --build
```

启动后访问：

| 服务 | 地址 |
|---|---|
| 前端 | http://localhost:5173 |
| 后端 API / Swagger | http://localhost:7176/swagger |
| 数据库 | `localhost:1433`，用户 `sa`，密码 `WiseWMS_Pass123` |

首次启动时，EF Core 会自动创建数据库并注入种子数据（默认管理员账号、默认供应商/客户、4 个分类）。

停止并清理（保留数据库数据）：

```bash
docker-compose down
```

彻底清理（包括数据库数据）：

```bash
docker-compose down -v
```

---

### 方式二：本地开发（不使用 Docker）

#### 前置依赖

- .NET SDK 8.0+
- Node.js 20+
- SQL Server（本地版或 LocalDB）

#### 1. 启动后端

```bash
cd WiseWMS.Api

# 还原依赖
dotnet restore

# 创建数据库（执行 EF Core 迁移）
dotnet tool install --global dotnet-ef
dotnet ef database update --project WiseWMS.Infrastructure --startup-project WiseWMS.Api

# 启动 API（默认监听 http://localhost:5173 以外端口，查看控制台输出）
dotnet run --project WiseWMS.Api
```

#### 2. 启动前端

```bash
cd wise-wms-web

# 安装依赖
npm install

# 开发模式启动（Vite 提供热更新）
npm run dev
```

默认访问：http://localhost:5173

如果后端 API 地址不是 `http://localhost:7176`，修改 `wise-wms-web/src/api/request.js` 中的 `baseURL`。

---

## 默认账号

首次启动后自动创建以下账号：

| 用户名 | 密码 | 角色 |
|---|---|---|
| `admin` | `admin123` | Admin |

同时会创建：默认供应商、默认客户、4 个默认分类（食品饮料 / 日用品 / 电子数码 / 办公用品）。

---

## 数据库设计

核心实体关系：

```
User
  └─ 操作人（关联入库单/出库单/库存流水）

Category  1 ── n  Product  1 ── n  InventoryTransaction
                      │
                      │
          ┌───────────┴───────────┐
          │                        │
  Supplier 1 ── n  InboundOrder   1 ── n  InboundItem   n ── 1  Product
                                     │
                                     └─ 入库时自动：
                                        ① Product.Stock += quantity
                                        ② InventoryTransaction (+Quantity)

  Customer 1 ── n  OutboundOrder  1 ── n  OutboundItem  n ── 1  Product
                                     │
                                     └─ 出库时自动：
                                        ① Product.Stock -= quantity
                                        ② InventoryTransaction (-Quantity)
```

---

## 认证与安全

- **JWT Bearer 认证**：登录接口 `/api/Auth/Login` 返回 Token，前端保存到 `localStorage`，Axios 拦截器自动注入到后续请求的 `Authorization: Bearer <token>` 头。
- **BCrypt 密码哈希**：数据库不存明文密码，只存哈希值。
- **[Authorize] 保护**：所有业务接口（商品、入库、出库、库存、仪表盘等）都需要认证，未登录请求返回 401，前端拦截后跳转登录页。
- **CORS 限制**：默认只允许 `http://localhost:5173` 跨域访问 API。

---

## API 接口一览

| 模块 | 方法 | 路径 | 说明 |
|---|---|---|---|
| 认证 | POST | `/api/Auth/Login` | 登录，返回 JWT Token |
| 商品 | GET | `/api/Products?keyword=&page=&pageSize=` | 分页查询商品 |
| 商品 | GET | `/api/Products/{id}` | 获取单个商品详情 |
| 商品 | POST | `/api/Products` | 新增商品 |
| 商品 | PUT | `/api/Products/{id}` | 修改商品 |
| 商品 | DELETE | `/api/Products/{id}` | 删除商品 |
| 供应商 | GET/POST/PUT/DELETE | `/api/Suppliers/...` | 供应商 CRUD |
| 客户 | GET/POST/PUT/DELETE | `/api/Customers/...` | 客户 CRUD |
| 入库 | GET | `/api/InboundOrders?keyword=&page=&pageSize=` | 入库单列表 |
| 入库 | GET | `/api/InboundOrders/{id}` | 入库单详情（含明细） |
| 入库 | POST | `/api/InboundOrders` | 新建入库单（事务 + 自动更新库存 + 流水） |
| 出库 | GET/POST | `/api/OutboundOrders/{id}` | 出库单 CRUD（事务 + 自动扣减 + 流水） |
| 库存 | GET | `/api/Inventory?keyword=&page=&pageSize=` | 实时库存查询 |
| 库存 | GET | `/api/Inventory/LowStock` | 低库存商品 |
| 仪表盘 | GET | `/api/Dashboard` | 统计数据（今日/累计出入库金额、总数等） |

完整 Swagger 文档：启动后端后访问 http://localhost:7176/swagger

---

## 常见问题

### Q1：首次 `docker-compose up` 后 API 容器报数据库连接失败？

A：SQL Server 容器首次启动较慢，API 可能在数据库就绪之前尝试连接。等待 30 秒后，重启 API 容器即可：

```bash
docker-compose restart api
```

### Q2：Docker 启动后前端页面正常，但请求 API 报跨域或网络错误？

A：检查 `wise-wms-web/src/api/request.js` 的 `baseURL` 是否为 `http://localhost:7176`（本地开发）。Docker 环境下前端已通过 `build.args.VITE_API_URL` 在构建时注入正确地址。

### Q3：如何清库重置？

```bash
docker-compose down -v   # 删除容器和数据卷（数据库数据会丢失）
docker-compose up -d --build
```

### Q4：本地开发如何加新的迁移？

```bash
dotnet ef migrations add YourMigrationName \
  --project WiseWMS.Infrastructure \
  --startup-project WiseWMS.Api
```

---

## License

本项目为学习/示例项目。
