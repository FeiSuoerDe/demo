# MCP工具使用文档

## 概述
MCP (MagicFarmTales Configuration Protocol) 工具集成了阿里云DevOps MCP服务，提供组织管理、项目管理、代码管理、流水线和制品仓库等全方位开发协作功能。

## 快速开始

### 基本使用流程
1. **获取组织信息**: 使用 `get_current_organization_info` 获取当前组织ID
2. **获取项目信息**: 使用 `search_projects` 查找目标项目和空间ID
3. **获取工作项类型**: 使用 `get_work_item_types` 获取正确的工作项类型ID
4. **创建工作项**: 使用 `create_work_item` 创建需求、任务等工作项
5. **建立关联关系**: 通过 `parentWorkitemId` 和 `sprintId` 建立父子关系和迭代关联

### 重要参数说明
- **organizationId**: 组织ID，通过 `get_current_organization_info` 获取
- **spaceId**: 项目空间ID，通过 `search_projects` 获取
- **workitemTypeId**: 工作项类型ID，通过 `get_work_item_types` 获取
- **assignedTo**: 指派人ID，通过 `search_organization_members` 获取
- **sprintId**: 迭代ID，关联工作项到特定迭代
- **parentWorkitemId**: 父工作项ID，用于建立父子关系

### 快速参考表

| 参数名 | 类型 | 必填 | 说明 | 获取方式 |
|--------|------|------|------|----------|
| organizationId | string | ✓ | 组织ID | `get_current_organization_info` |
| spaceId | string | ✓ | 项目空间ID | `search_projects` |
| workitemTypeId | string | ✓ | 工作项类型ID | `get_work_item_types` |
| subject | string | ✓ | 工作项标题 | 手动输入 |
| assignedTo | string | ✗ | 指派人ID | `search_organization_members` |
| description | string | ✗ | 工作项描述 | 手动输入 |
| sprintId | string | ✗ | 迭代ID | 项目管理界面查看 |
| parentWorkitemId | string | ✗ | 父工作项ID | 创建父工作项后获取 |
| repositoryId | string | ✓ | 代码仓库ID | `list_repositories` |
| branch | string | ✓ | 分支名称 | 手动输入 |
| filePath | string | ✓ | 文件路径 | 手动输入（使用/分隔符） |

## 工具分类与功能说明

### 1. 组织管理工具
- `get_current_organization_info`: 获取当前组织信息
- `get_user_organizations`: 获取用户加入的组织列表
- `get_organization_role`: 获取组织角色信息
- `get_organization_departments`: 获取组织部门列表
- `get_organization_department_info`: 获取部门详情
- `get_organization_department_ancestors`: 获取部门上级部门
- `list_organization_members`: 获取组织成员列表
- `get_organization_member_info`: 获取成员信息
- `search_organization_members`: 搜索组织成员
- `list_organization_roles`: 列出组织角色
- `get_organization_role`: 获取角色详情

### 2. 项目管理工具
- `get_project`: 获取项目详情
- `search_projects`: 搜索项目
- `get_work_item`: 获取工作项详情
- `search_workitems`: 搜索工作项
- `get_work_item_types`: 获取工作项类型
- `create_work_item`: 创建工作项

### 3. 代码管理工具
- **分支操作**
  - `create_branch`: 创建分支
  - `delete_branch`: 删除分支
  - `get_branch`: 获取分支信息
  - `list_branches`: 列出分支
- **文件操作**
  - `create_file`: 创建文件
  - `delete_file`: 删除文件
  - `get_file_blobs`: 获取文件内容
  - `update_file`: 更新文件
  - `list_files`: 查询文件树
- **合并请求**
  - `create_change_request`: 创建变更请求
  - `list_change_requests`: 列出变更请求
  - `get_change_request`: 获取变更请求详情
  - `create_change_request_comment`: 添加变更请求评论

### 4. 流水线工具
- `get_pipeline`: 获取流水线详情
- `list_pipelines`: 列出流水线
- `create_pipeline_from_description`: 根据描述创建流水线
- `run_pipeline`: 运行流水线
- `get_pipeline_run_log`: 获取流水线运行日志
- `list_pipeline_tasks`: 获取流水线任务列表

### 5. 制品仓库工具
- `list_package_repositories`: 查看制品仓库
- `list_artifacts`: 查询制品信息
- `get_artifact`: 获取制品详情

### 6. 其他工具
- `create_change_request_comment`: 创建变更请求评论
- `list_change_request_comments`: 列出变更请求评论
- `list_change_request_patch_sets`: 列出变更请求补丁集

## 详细使用示例

### 1. 获取组织信息
```json
get_current_organization_info {}
```
**返回示例**:
```json
{
  "id": "66cc5350eb9e5d287c38****",
  "name": "组织名称"
}
```

### 2. 搜索项目
```json
search_projects {
  "organizationId": "66cc5350eb9e5d287c38****",
  "keyword": "MagicFarmTales"
}
```

### 3. 获取工作项类型
```json
get_work_item_types {
  "organizationId": "66cc5350eb9e5d287c38****",
  "spaceId": "2777****",
  "category": "Req"  // 可选值: ["Req","Risk","Bug","Task","Request","Topic"]
}
```
**返回示例**:
```json
[
  {
    "id": "61db9af2148974246176****",
    "name": "产品类需求",
    "nameEn": "Requirement"
  },
  {
    "id": "61db9af2148974246176****",
    "name": "任务",
    "nameEn": "Task"
  }
]
```

### 4. 创建工作项（需求）
```json
create_work_item {
  "organizationId": "66cc5350eb9e5d287c38****",
  "spaceId": "2777****",
  "subject": "美术素材生产",
  "workitemTypeId": "61db9af2148974246176****",
  "assignedTo": "66cc5350eb9e5d287c38****",
  "description": "负责游戏中所有美术素材的制作和管理",
  "sprintId": "2777158"  // 可选，关联到迭代
}
```

### 5. 创建子任务
```json
create_work_item {
  "organizationId": "66cc5350eb9e5d287c38****",
  "spaceId": "2777****",
  "subject": "工具类素材制作",
  "workitemTypeId": "61db9af2148974246176****",  // 任务类型ID
  "assignedTo": "66cc5350eb9e5d287c38****",
  "description": "制作农具、工具等相关素材",
  "parentWorkitemId": "13518****",  // 父工作项ID
  "sprintId": "2777158"
}
```

### 6. 搜索工作项
```json
search_workitems {
  "organizationId": "66cc5350eb9e5d287c38****",
  "spaceId": "2777****",
  "category": "Req",  // 按类型筛选
  "keyword": "美术"  // 关键词搜索
}
```

### 7. 创建分支示例
```json
create_branch {
  "organizationId": "66cc5350eb9e5d287c38****",
  "repositoryId": "2835387",
  "branch": "feature/new-crop-system",
  "ref": "master"
}
```

### 8. 创建文件示例
```json
create_file {
  "organizationId": "66cc5350eb9e5d287c38****",
  "repositoryId": "2835387",
  "filePath": "/Scripts/Farm/NewFeature.cs",
  "content": "// 新功能代码",
  "commitMessage": "添加新功能",
  "branch": "feature/new-crop-system"
}
```

## 常见错误及解决方案

### 1. 工作项类型错误
**错误**: `Invalid input: workitemTypeId is required`
**解决**: 必须先调用 `get_work_item_types` 获取正确的工作项类型ID

### 2. 组织ID或空间ID错误
**错误**: `Organization not found` 或 `Space not found`
**解决**: 使用 `get_current_organization_info` 和 `search_projects` 获取正确的ID

### 3. 父工作项关联失败
**错误**: `Parent workitem not found`
**解决**: 确保父工作项ID存在且有权限访问

### 4. 迭代关联失败
**错误**: `Sprint not found`
**解决**: 确认迭代ID正确，可通过项目管理界面查看

## 最佳实践

### 1. 工作项创建流程
1. **规划阶段**: 先创建需求类工作项作为主要功能模块
2. **分解阶段**: 将需求分解为具体的任务
3. **关联阶段**: 建立父子关系和迭代关联
4. **指派阶段**: 为每个任务指派负责人

### 2. 命名规范
- **需求**: 使用功能模块名称，如"美术素材生产"、"种植系统开发"
- **任务**: 使用具体任务描述，如"工具类素材制作"、"作物生长逻辑实现"
- **分支**: 使用 `feature/功能名称` 格式

### 3. 描述规范
- 提供清晰的功能描述和验收标准
- 包含技术实现要点
- 标注依赖关系和前置条件

### 4. 批量操作建议
- 先获取所有必要的ID信息
- 按照依赖关系顺序创建工作项
- 及时验证创建结果

## 工具调用限制

### 当前MCP工具集限制
- **不支持**: 更新工作项（`update_work_item`）
- **不支持**: 删除工作项（`delete_work_item`）
- **替代方案**: 重新创建工作项并建立正确关联

### 权限要求
- 组织成员权限
- 项目访问权限
- 代码仓库读写权限

## 注意事项
1. 所有操作需提供有效的组织ID和仓库ID
2. 敏感操作（删除/修改）前请确认权限和备份状态
3. API调用时需使用URL编码格式传递特殊字符路径
4. 工作项类型ID在不同组织中可能不同，需要动态获取
5. 创建工作项时建议同时指定迭代和指派人，避免后续修改困难
6. 文件路径必须使用正斜杠（/），即使在Windows环境下