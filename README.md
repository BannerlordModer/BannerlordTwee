# BannerlordTwee
在骑砍2中加载twee文件当作对话。

## 功能
本项目包含两个核心部分：
1.  **Twee 解析器**: 一个用于解析 Twee 3 格式文件的 .NET 库。
2.  **对话源生成器**: 一个 Roslyn Source Generator，它会在编译时自动将 `.twee` 文件转换为《骑马与砍杀2：霸主》可用的对话 C# 代码。

## 使用方法

1.  在你的 Bannerlord Mod 项目中，添加对 `BannerlordTwee.SourceGenerator` 的项目引用。
2.  将你的 `.twee` 对话文件添加到你的项目中。
3.  在 `.csproj` 文件中，将这些 `.twee` 文件标记为 `AdditionalFiles`，如下所示：
    ```xml
    <ItemGroup>
      <AdditionalFiles Include="MyDialogs\village_mission.twee" />
    </ItemGroup>
    ```
4.  编译器会自动为你生成一个 `partial class`，其名称基于 `.twee` 文件名（例如，`village_mission.twee` 会生成 `VillageMissionDialogs` 类）。
5.  创建一个新的 C# 文件，在其中实现这个 `partial class` 的另一部分，并填充对话所需的条件和结果逻辑。

## 配置

### 自定义命名空间
默认情况下，生成的代码位于 `BannerlordTwee.Generated.Dialogs` 命名空间中。你可以通过在你的 `.csproj` 文件中添加 `BannerlordTweeNamespace` 属性来自定义它：
```xml
<PropertyGroup>
  ...
  <BannerlordTweeNamespace>MyMod.CustomDialogs</BannerlordTweeNamespace>
</PropertyGroup>
```

## Twee 到 Bannerlord 对话 API 映射

源生成器会将 Twee 的结构映射到 Bannerlord 的对话 API。

### Twee 结构
一个典型的 Twee 文件由多个段落（Passage）组成，每个段落以 `::` 开头，后跟标题和可选的标签。

```twee
:: 开始
你好，旅行者。

[[我该去哪里？->前往村庄]]
[[再见]]
```

### Bannerlord API
生成器主要使用 `CampaignGameStarter` 的两个扩展方法：
-   `AddDialogLine`: 用于添加一句 NPC 的对话。
-   `AddPlayerLine`: 用于添加一个玩家的回复选项。

### 映射规则
-   **Twee 文件** -> **静态对话类**:
    -   一个名为 `dialog.twee` 的文件会生成一个名为 `DialogDialogs` 的 `public static partial` 类。

-   **段落 (Passage)** -> **NPC 对话 (`AddDialogLine`)**:
    -   `:: 开始` 这个段落会被转换成一个 `AddDialogLine` 调用。
    -   **id / input_token / output_token**: 会根据文件名和段落名生成唯一的标识符，例如 `dialog_开始`, `dialog_开始_start`, `dialog_开始_player_options`。
    -   **文本**: 段落中移除了链接标记（`[[...]]`）后的内容。
    -   **条件委托**: 生成一个名为 `private static partial bool 开始Condition()` 的方法存根，需要你在 `partial` 类的另一部分实现它，用于决定这条对话是否出现。

-   **链接 (Link)** -> **玩家选项 (`AddPlayerLine`)**:
    -   `[[我该去哪里？->前往村庄]]` 这个链接会被转换成一个 `AddPlayerLine` 调用。
    -   **文本**: 链接中显示的文本，即 `我该去哪里？`。
    -   **目标**: 链接指向的目标段落（`前往村庄`）决定了点击该选项后对话会跳转到哪里。
    -   **条件与结果委托**: 生成两个方法存根，`private static partial bool 开始_前往村庄LinkCondition()` 和 `private static partial void 开始_前往村庄LinkConsequence()`，需要你来实现，分别用于决定该选项是否可见以及点击后执行的逻辑。
