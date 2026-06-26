# WiseWMS 进销存管理系统

基于 **.NET 8 + Vue 3** 的进销存管理系统，采用三层架构，集成 Redis 缓存、RabbitMQ 消息队列与 Docker 容器化部署，覆盖商品、供应商、客户、入库、出库、库存、仪表盘等核心业务流程，配备 52 个单元测试与 GitHub Actions CI 流水线。

---

## 技术栈

### 后端

| 技术 | 用途 |
|---|---|
| ASP.NET Core 8 Web API | RESTful API 框架 |
| 三层架构（API / Application / Infrastructure） | 职责分离，业务逻辑与数据访问解耦 |
| Entity Framework Core 8 | ORM 与数据库迁移 |
| SQL Server 2022 | 关系型数据库 |
| Redis 7 | 分布式缓存（商品列表、分类、仪表盘统计） |
| RabbitMQ 3 | 消息队列（库存变动异步通知 + 缓存失效） |
| JWT Bearer | 无状态认证（120 分钟有效期） |
| BCrypt.Net-Next | 密码哈希 |
| FluentValidation | 请求参数校验（9 个验证器） |
| AutoMapper | DTO 与实体映射 |
| Serilog | 结构化日志（控制台 + 按天滚动文件） |
| ASP.NET Core Rate Limiting | 登录接口限流（5 次/分钟） |
| Asp.Versioning.Mvc | API 版本管理（v1.0） |
| ClosedXML | Excel 导出（.xlsx） |
| Health Checks | 服务健康检查（`/health`） |
| Swagger + OpenAPI | 接口文档与 Bearer 鉴权 |

### 前端

| 技术 | 用途 |
|---|---|
| Vue 3.5 | 组合式 API + `<script setup>` |
| Element Plus 2.14 | UI 组件库（中文语言包） |
| Vue Router | Hash 路由模式 + 导航守卫 |
| Axios | HTTP 请求，统一拦截器（JWT 注入 + 401 跳转） |
| Vite 8 | 构建工具与热更新 |
| Nginx | 生产环境静态资源托管 |

### 部署与 CI/CD

- **Docker Compose**：五容器编排（SQL Server / Redis / RabbitMQ / API / Web）
- **GitHub Actions**：push/PR 触发 `restore → build → test → Docker build`
- **Dockerfile**：前后端均多阶段构建，环境变量注入配置

---

## 功能模块

| 模块 | 核心能力 |
|---|---|
| 🔐 登录认证 | JWT Token 登录、Token 自动注入、401 自动跳转、登录限流 |
| 🏠 仪表盘 | 商品总数、库存总量、今日入/出库、低库存预警（Redis 缓存 30s） |
| 📦 商品管理 | CRUD、搜索分页、分类关联、Excel 导出、Redis 缓存（10min） |
| 🏷️ 分类管理 | 列表、新增、删除（Admin 权限）、Redis 缓存（10min） |
| 🏢 供应商管理 | CRUD、搜索分页、引用完整性校验 |
| 👥 客户管理 | CRUD、搜索分页、引用完整性校验 |
| 📥 入库管理 | 主子表新建、自动增加库存、自动生成库存流水、订单号自动生成 |
| 📤 出库管理 | 主子表新建、库存不足拦截、自动扣减库存与流水、订单号自动生成 |
| 📊 库存管理 | 实时库存查询、库存流水追溯（独立分页）、低库存预警（三色状态标签） |

---

## 项目结构

```
WiseWMS/
├── WiseWMS.Api/                              # 后端解决方案
│   ├── WiseWMS.Api/                          #   API 层：控制器、中间件、后台服务
│   │   ├── Controllers/                      #   9 个控制器
│   │   ├── Middleware/                       #   全局异常中间件
│   │   ├── BackgroundServices/               #   RabbitMQ 消费者（InventorySyncConsumer）
│   │   ├── Program.cs                        #   启动配置 + DI 组装
│   │   ├── appsettings.json
│   │   └── Dockerfile
│   ├── WiseWMS.Application/                  #   业务逻辑层
│   │   ├── DTOs/                             #   20 个 DTO
│   │   ├── Services/                         #   9 个 Service 实现
│   │   ├── Services/Interfaces/              #   9 个 Service 接口
│   │   ├── Validators/                       #   9 个 FluentValidation 验证器
│   │   ├── Profiles/                         #   AutoMapper 映射配置
│   │   └── Publishers/                       #   RabbitMQ 消息发布器
│   ├── WiseWMS.Infrastructure/               #   数据访问层
│   │   ├── Data/                             #   DbContext + 种子数据
│   │   ├── Entities/                         #   10 个实体
│   │   ├── Migrations/                       #   2 个迁移
│   │   └── MessageQueue/                     #   RabbitMQ 连接封装
│   ├── tests/
│   │   └── WiseWMS.UnitTests/                #   52 个单元测试
│   └── WiseWMS.Api.slnx
├── wise-wms-web/                             # 前端项目
│   ├── src/
│   │   ├── api/                              #   Axios 封装 + 10 个模块 API
│   │   ├── views/                            #   9 个页面组件
│   │   ├── layouts/                          #   主布局（侧边栏 + 内容区）
│   │   ├── router/                           #   路由配置 + 导航守卫
│   │   ├── main.js
│   │   └── App.vue
│   ├── Dockerfile
│   └── package.json
├── .github/workflows/ci.yml                  # GitHub Actions CI
├── docker-compose.yml                        # 五容器编排
└── README.md
```

### 后端分层职责

| 项目 | 职责 |
|---|---|
| `WiseWMS.Api` | 接收 HTTP 请求、认证授权、参数绑定、全局异常处理、后台服务 |
| `WiseWMS.Application` | 业务规则、事务控制、DTO 映射、输入校验、消息发布 |
| `WiseWMS.Infrastructure` | EF Core DbContext、实体定义、数据库迁移、种子数据、RabbitMQ 连接 |

---

## 架构亮点

### 1. 分层架构

控制器只负责 HTTP 语义，业务规则封装在 `Application.Services`，数据访问收敛在 `Infrastructure`，符合单一职责原则，便于单元测试与后续演进。

### 2. 认证与授权

- JWT Bearer Token，支持 `ValidateIssuer / Audience / Lifetime / IssuerSigningKey`
- 密码使用 BCrypt 哈希存储
- 所有业务控制器标注 `[Authorize]`，删除接口额外要求 `[Authorize(Roles = "Admin")]`
- Token 有效期 120 分钟

### 3. 事务安全

入库/出库创建时启用 `BeginTransactionAsync()`，订单创建、库存增减、流水写入在同一事务中提交，失败自动回滚。出库时实时校验库存充足性，不足则抛出业务异常。

### 4. Redis 缓存策略

| 缓存键 | 服务 | TTL | 失效方式 |
|---|---|---|---|
| `products_all` | ProductService | 10 分钟 | 增删改时主动失效 + RabbitMQ 消费者失效 |
| `categories_all` | CategoryService | 10 分钟 | 增删时主动失效 |
| `dashboard_stats` | DashboardService | 30 秒 | RabbitMQ 消费者失效 |

### 5. RabbitMQ 消息队列

入库/出库操作事务提交后，通过 `MessagePublisher` 发布库存变动消息至 `inventory_sync` 队列。`InventorySyncConsumer` 后台服务消费消息，清除 Redis 缓存，实现缓存与数据库的最终一致性。

```
入库/出库事务提交
       │
       ▼
  MessagePublisher ──► RabbitMQ (inventory_sync 队列)
                              │
                              ▼
                  InventorySyncConsumer (BackgroundService)
                              │
                              ▼
                  清除 Redis: products_all + dashboard_stats
```

### 6. 输入校验

- DTO 使用 `DataAnnotations` 做基础校验
- `FluentValidation` 做复杂规则校验（9 个验证器，覆盖字段长度、范围、必填等）
- 列表接口对 `pageSize` 做范围限制，防止全表拉取

### 7. 限流与防护

- 登录接口启用固定窗口限流：1 分钟最多 5 次
- 全局中间件注入安全响应头：`X-Content-Type-Options`、`X-Frame-Options`、`X-XSS-Protection`、`Referrer-Policy`

### 8. 可观测性

- Serilog 结构化日志输出到控制台与按天滚动文件（`logs/wisewms-*.log`）
- `/health` 健康检查端点
- 全局异常中间件统一捕获并返回标准化 JSON `{ code, message, data }`

---

## 数据库设计

核心实体关系：

```
User  ── 操作人（关联入库单 / 出库单 / 库存流水）

Category  1 ── n  Product  1 ── n  InventoryTransaction
                    │
        ┌───────────┴────────────┐
        │                        │
Supplier 1 ── n  InboundOrder  1 ── n  InboundItem  n ── 1  Product
                                  │
                                  └─ 入库时自动（事务内）：
                                     ① Product.Stock += quantity
                                     ② InventoryTransaction (+Quantity)

Customer 1 ── n  OutboundOrder 1 ── n  OutboundItem n ── 1  Product
                                  │
                                  └─ 出库时自动（事务内）：
                                     ① 校验库存充足性
                                     ② Product.Stock -= quantity
                                     ③ InventoryTransaction (-Quantity)
```

订单号自动生成规则：`IN-yyyyMMdd-0001`（入库）/ `OUT-yyyyMMdd-0001`（出库），按当日序号递增。

---

## 快速启动

### 方式一：Docker Compose（推荐）

项目根目录执行：

```bash
docker-compose up -d --build
```

五个服务会依次启动：

| 服务 | 端口 | 说明 |
|---|---|---|
| SQL Server 2022 | `localhost:1433` | 用户 `sa`，密码 `WiseWMS_Pass123` |
| Redis 7 | `localhost:6379` | 缓存服务 |
| RabbitMQ 3 | `localhost:5672` / `localhost:15672` | 消息队列 / 管理界面（guest/guest） |
| 后端 API | `localhost:7176` | Swagger: http://localhost:7176/swagger |
| 前端 Web | `localhost:5173` | Nginx 静态托管 |

首次启动会自动创建数据库并注入种子数据。

停止：

```bash
docker-compose down
```

彻底清理（含数据库数据）：

```bash
docker-compose down -v
```

### 方式二：本地开发

#### 前置依赖

- .NET SDK 8.0+
- Node.js 20+
- SQL Server / LocalDB
- Redis（可选，缺失时降级为无缓存）
- RabbitMQ（可选，缺失时降级为无消息通知）

#### 启动后端

```bash
cd WiseWMS.Api

# 还原依赖
dotnet restore WiseWMS.Api.slnx

# 安装 EF Core CLI（如未安装）
dotnet tool install --global dotnet-ef

# 创建数据库
dotnet ef database update \
  --project WiseWMS.Infrastructure \
  --startup-project WiseWMS.Api \
  --solution-dir .

# 启动 API
dotnet run --project WiseWMS.Api
```

#### 启动前端

```bash
cd wise-wms-web

# 安装依赖
npm install

# 开发模式启动
npm run dev
```

默认访问 http://localhost:5173。

前端默认连接 `https://localhost:7176`，如需修改后端地址，创建 `.env.local` 文件：

```
VITE_API_URL=http://localhost:5202
```

#### 运行测试

```bash
cd WiseWMS.Api
dotnet test WiseWMS.Api.slnx
```

---

## 默认账号

首次启动后自动注入：

| 用户名 | 密码 | 角色 |
|---|---|---|
| `admin` | `admin123` | Admin |

同时创建 1 个默认供应商、1 个默认客户、4 个默认分类（食品饮料 / 日用品 / 电子数码 / 办公用品）。

---

## API 接口一览

完整 Swagger 文档：http://localhost:7176/swagger

| 模块 | 方法 | 路径 | 说明 |
|---|---|---|---|
| 认证 | POST | `/api/Auth/login` | 登录，返回 JWT Token |
| 商品 | GET | `/api/Products?keyword=&page=&pageSize=` | 分页查询 |
| 商品 | GET | `/api/Products/{id}` | 商品详情 |
| 商品 | POST | `/api/Products` | 新增商品 |
| 商品 | PUT | `/api/Products/{id}` | 修改商品 |
| 商品 | DELETE | `/api/Products/{id}` | 删除商品（Admin） |
| 商品 | GET | `/api/Products/export` | 导出 Excel |
| 分类 | GET | `/api/Categories` | 分类列表 |
| 分类 | POST | `/api/Categories` | 新增分类（Admin） |
| 分类 | DELETE | `/api/Categories/{id}` | 删除分类（Admin） |
| 供应商 | GET | `/api/Suppliers?keyword=&page=&pageSize=` | 分页查询 |
| 供应商 | GET | `/api/Suppliers/{id}` | 供应商详情 |
| 供应商 | POST | `/api/Suppliers` | 新增供应商 |
| 供应商 | PUT | `/api/Suppliers/{id}` | 修改供应商 |
| 供应商 | DELETE | `/api/Suppliers/{id}` | 删除供应商（Admin） |
| 客户 | GET | `/api/Customers?keyword=&page=&pageSize=` | 分页查询 |
| 客户 | GET | `/api/Customers/{id}` | 客户详情 |
| 客户 | POST | `/api/Customers` | 新增客户 |
| 客户 | PUT | `/api/Customers/{id}` | 修改客户 |
| 客户 | DELETE | `/api/Customers/{id}` | 删除客户（Admin） |
| 入库 | GET | `/api/InboundOrders?keyword=&page=&pageSize=` | 入库单列表 |
| 入库 | GET | `/api/InboundOrders/{id}` | 入库单详情（含明细） |
| 入库 | POST | `/api/InboundOrders` | 新建入库单 |
| 出库 | GET | `/api/OutboundOrder?keyword=&page=&pageSize=` | 出库单列表 |
| 出库 | GET | `/api/OutboundOrder/{id}` | 出库单详情（含明细） |
| 出库 | POST | `/api/OutboundOrder` | 新建出库单 |
| 库存 | GET | `/api/Inventory/transactions?productId=&page=&pageSize=` | 库存流水 |
| 库存 | GET | `/api/Inventory/low-stock` | 低库存商品 |
| 仪表盘 | GET | `/api/Dashboard` | 统计数据 |

---

## 单元测试

52 个单元测试覆盖全部 9 个业务服务，使用 xUnit + Moq + EF Core InMemory：

| 测试文件 | 测试数 | 覆盖内容 |
|---|---|---|
| AuthServiceTests | 3 | 有效/无效登录、用户不存在 |
| ProductServiceTests | 11 | CRUD、关键字搜索、重复名称校验 |
| SupplierServiceTests | 10 | CRUD、关键字搜索 |
| CustomerServiceTests | 7 | CRUD、关键字搜索 |
| CategoryServiceTests | 6 | 列表、新增、重复校验、删除（含引用检查） |
| InboundServiceTests | 4 | 入库创建（库存增加）、商品不存在、列表查询 |
| OutboundServiceTests | 5 | 出库创建（库存扣减）、库存不足拦截、商品不存在 |
| InventoryServiceTests | 4 | 流水查询、低库存查询 |
| DashboardServiceTests | 2 | 空数据库、有数据的统计 |

---

## CI/CD

GitHub Actions 流水线（`.github/workflows/ci.yml`），在 push/PR 到 main/master 时触发：

```
checkout → setup .NET 8 → restore → build (Release) → test → Docker build
```

---

## 常见问题

### Q1：首次 `docker-compose up` 后 API 报数据库连接失败？

SQL Server 容器首次启动较慢，API 可能提前尝试连接。等待 30 秒后重启 API 容器：

```bash
docker-compose restart api
```

### Q2：前端页面正常，但请求 API 报跨域或网络错误？

确保浏览器地址是 `http://localhost:5173`。Docker 构建时通过 `VITE_API_URL` 注入后端地址 `http://localhost:7176`，若后端端口改动需重新构建 `web` 服务。

### Q3：如何清库重置？

```bash
docker-compose down -v
docker-compose up -d --build
```

### Q4：本地开发如何添加新迁移？

```bash
dotnet ef migrations add YourMigrationName \
  --project WiseWMS.Infrastructure \
  --startup-project WiseWMS.Api \
  --solution-dir .
```

### Q5：本地开发没有 Redis / RabbitMQ 怎么办？

后端会正常启动，但缓存和消息通知功能不可用。如需完整体验，可用 Docker 单独启动：

```bash
docker run -d -p 6379:6379 redis:7-alpine
docker run -d -p 5672:5672 -p 15672:15672 rabbitmq:3-management-alpine
```

---

## License

本项目为示例项目。
