# Handoff para o Claude — 2026-08-21 01:10 BRT

Continue do estado atual; não reinicialize o projeto e não reverta os commits abaixo.

## O que acabou de ser entregue

1. `fe1c5d8 feat(ui): add research company character and event modal`
   - Research, Company e Character integradas ao shell.
   - Modal de eventos com escolhas reais.
   - `PendingEvent` não é mais sobrescrito antes da resposta.
2. `e792d68 fix(products): enforce tested lifecycle before launch`
   - Lançamento bloqueado antes de Development completo + Teste.
   - Bugs ocultos (`BugCount`) separados dos descobertos (`KnownBugCount`).
   - Correção atua somente nos descobertos.
   - Save schema V2 e migração V1→V2.
3. Incremento final desta sessão: ícone original Android integrado e APK reconstruído. Consulte `git log -3 --oneline` para o hash do commit.

## Evidência verificada

- Domínio: 90/90 (`dotnet test Tests.NET/StartupEmpire.Domain.Tests.csproj`).
- Backend: 22/22 (`dotnet test backend/StartupEmpire.Api.Tests/StartupEmpire.Api.Tests.csproj`); dentro de sandbox o Windows Event Log causa falso negativo, portanto rode com permissão normal.
- Unity EditMode: 27/27.
- Unity PlayMode: 7/7.
- APK: `Builds/Android/StartupEmpire-debug.apk` (ignorado pelo Git), 45.648.468 bytes.
- SHA-256: `CDE073EE732B03957F68801E25C4723A0E4B21801822A23C972BE42B3374A6BF`.
- BuildReport final: `Succeeded`, 0 erros, 0 avisos.
- `aapt2 dump badging` confirmou o ícone em ldpi/mdpi/hdpi/xhdpi/xxhdpi/xxxhdpi.
- O APK ainda NÃO foi executado em aparelho; o emulador visto na auditoria estava offline.

## Arquivos centrais alterados

- `Assets/Game/UI/EventModalBuilder.cs`
- `Assets/Game/UI/Screens/{Research,Company,Character,Office,Products}ScreenPanel.cs`
- `Assets/Game/Products/{ProductState,DevelopmentService}.cs`
- `Assets/Game/Save/{SaveDataV1,SaveMigrator,SaveService}.cs`
- `Assets/Game/Art/StartupEmpireAppIcon.png`
- `Assets/Game/EditorTools/AndroidBuilder.cs`
- `PROGRESS.md`, `PROJECT-PLAN.md`, `CHANGELOG.md`

## Próximos P0 recomendados

1. Implementar dias/ciclos de trabalho persistidos: estudar, desenvolver, testar e corrigir devem consumir recurso limitado; "Encerrar dia" executa o ciclo econômico uma vez.
2. Corrigir UX Android portrait: safe area, no máximo 4–5 destinos principais + menu "Mais", alvos de toque adequados e botão Voltar.
3. Criar Splash/Main Menu/New Game/Continue e tutorial contextual persistido.
4. Ligar emulador/dispositivo, instalar o APK atual e fazer smoke test + logcat antes de declarar runtime Android validado.

## Limitações que não devem ser esquecidas

- O teste do modal ainda usa sorteio probabilístico (300 ciclos); tornar o RNG injetável seria melhor.
- Save corrompido ainda cai em jogo novo sem backup `.bak` recuperável.
- Backend não tem autenticação/ownership comercial; `PlayerId` ainda é auto-declarado.
- Não existe AAB/release signing/CI; o APK atual é debug.
- A maior parte da UI interna ainda é placeholder e não respeita safe area.

Antes de continuar, rode `git status --short --branch` e leia `PROGRESS.md`. A working tree deve estar limpa após o commit final desta sessão.
