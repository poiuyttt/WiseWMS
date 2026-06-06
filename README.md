# WiseWMS 进销存管理系统

基于 .NET 8 + Vue 3 的企业级进销存管理系统，支持商品管理、入库/出库管理、库存查询与流水追踪。

## 技术栈

- **后端**：ASP.NET Core 8 Web API (三层架构)
- **前端**：Vue 3 + Element Plus + Pinia (即将开发)
- **ORM**：Entity Framework Core
- **数据库**：SQL Server
- **认证**：JWT + BCrypt

## 功能模块

- [x] JWT 登录认证 + 角色权限
- [x] 商品管理 CRUD + 分页搜索
- [x] 入库单管理（含库存更新）
- [ ] 出库单管理（含库存扣减）
- [ ] 库存查询 + 库存流水
- [ ] Dashboard 图表
- [ ] Excel 导出

## 项目结构

```
WiseWMS.Api           → WebAPI 控制器 + 中间件
WiseWMS.Application   → Service 业务逻辑 + DTO + 校验
WiseWMS.Infrastructure→ EF Core + 实体 + Repository
```

## 快速启动

```bash
# 1. 配置连接字符串 (appsettings.json)
# 2. 安装 dotnet ef 工具
dotnet tool install --global dotnet-ef

# 3. 创建数据库
dotnet ef database update --project WiseWMS.Infrastructure --startup-project WiseWMS.Api

# 4. 启动 API
dotnet run --project WiseWMS.Api
```

默认管理员账号：`admin` / `admin123`
