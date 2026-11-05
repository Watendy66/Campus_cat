# 校园猫猫观察日记

Unity 2018.4.36f1 / Windows / 内置渲染管线

## 运行
1. 解压后在 Unity 中打开本目录，等待首次导入完成。
2. 打开 Assets/CampusCats/Generated/Campus.unity 并按 Play。
3. 在主菜单选择角色。已有场景、配置与 Prefab 已接好，无需重新生成场景。

WASD 移动，Shift 奔跑，Ctrl 蹲下，E 交互，1/2 切换道具，Q 使用。
右键摄影，左键快门，滚轮缩放，J 手账，P 相册，Esc 暂停。
任务可在暂停或手账中跳过。结尾阅读照料记录，准备三猫合照，再点击快门或按空格。
音乐可在设置中调节和静音。本局不跨启动保存，重新开始会清空任务与相册。

## 构建
菜单“校园猫猫 / 构建 Windows 试玩版”将构建当前已完成的场景，不重建或覆盖地图。
需要编辑器已安装 Windows Build Support。

## 工程结构
Assets/CampusCats/Scripts 为完整运行脚本；Generated 为场景及配置；Art 为模型与环境；Resources 为纸纹、音乐和着色器。
Packages 与 ProjectSettings 随包保留。Library、Temp、日志、测试探针和录屏工具不属于运行依赖，未打包。
项目原有署名为 Campus Diary。第三方素材的许可保留在原目录，详见素材说明。

## 验证范围
此前当前场景已完成正常主线运行，20项流程检查通过且未跳过任务；结尾与音乐17项检查通过。
本包另执行C#编译和资源引用核对；未重新执行从干净缓存导入后的独立Player全流程验收。
