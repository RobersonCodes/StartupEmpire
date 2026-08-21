# Handoff para o Claude — 2026-08-21 01:55 (America/Sao_Paulo)

Retome deste estado. Não reinicialize o projeto, não reverta commits e preserve alterações do usuário. Leia `PROGRESS.md`, `PROJECT-PLAN.md`, `GAME-DESIGN-DOCUMENT.md` e `ARCHITECTURE.md` antes de editar.

## Estado entregue neste handoff

- Splash → Main Menu → Continue/New Game implementados por `StartupFlowBuilder`.
- `Continuar` só habilita quando existia save no início da sessão.
- `Novo Jogo` exige confirmação antes de apagar progresso existente.
- Tutorial contextual do Capítulo 1 implementado no domínio e no Office: estudar → desenvolver → testar → corrigir bugs conhecidos → lançar → conquistar primeiro cliente.
- A ação sugerida recebe destaque visual; o progresso do tutorial não depende da UI.
- Save schema atual: **V4**. Migração de saves antigos preserva progresso e considera tutorial concluído quando já havia produto lançado/manutenção/descontinuado.
- PlayMode batch usa `InMemorySaveStorage`, portanto os testes não leem nem alteram o save real em `Application.persistentDataPath`.
- Ícone original, portrait, safe area e navegação mobile continuam integrados.

## Evidência verificada nesta sessão

- `dotnet test Tests.NET/StartupEmpire.Domain.Tests.csproj`: **92/92**, 0 falhas.
- Unity EditMode: **32/32**, 0 falhas (`TestResults/editmode.xml`, pasta ignorada pelo Git).
- Unity PlayMode: **9/9**, 0 falhas (`TestResults/playmode.xml`).
- Backend: **22/22**, 0 falhas.
- APK debug reconstruído com BuildReport `Succeeded`, 0 erros e 0 avisos.
- APK: `Builds/Android/StartupEmpire-debug.apk` (ignorado pelo Git), **45.617.747 bytes**.
- SHA-256: `8115D0D358499C72C495A4944E806ED5FA200BBEC3C35C71F1A331C0A1C90AB3`.
- `aapt2`: package `com.startupempire.game`, minSdk 23, targetSdk 36, `screenOrientation=1`.
- Runtime Android ainda **não validado**: `adb devices` retorna `emulator-5554 offline`.

## Arquivos principais deste incremento

- `Assets/Game/Core/TutorialStep.cs`
- `Assets/Game/Core/GameRoot.cs`
- `Assets/Game/Core/GameState.cs`
- `Assets/Game/UI/StartupFlowBuilder.cs`
- `Assets/Game/UI/GameShellBuilder.cs`
- `Assets/Game/UI/Screens/OfficeScreenPanel.cs`
- `Assets/Game/Save/{SaveDataV1,SaveMigrator,SaveService}.cs`
- `Assets/Game/Tests/{EditMode/SaveServiceTests,PlayMode/GameShellBuilderTests}.cs`

## Próximas prioridades recomendadas

1. Recuperar/recriar um emulador online ou conectar aparelho, instalar o APK e fazer smoke test com logcat. Não declarar runtime Android validado até isso ocorrer.
2. Implementar backup recuperável `.bak` para save corrompido, preservando o fallback atual e cobrindo com testes.
3. Criar tela dedicada de Development e melhorar estados bloqueados/scroll das listas.
4. Definir a unidade econômica de salários/receita por dia e rebalancear com testes determinísticos.
5. Gerar AAB e preparar assinatura release somente quando houver configuração segura de chave fora do repositório.

## Limitações conhecidas

- O emulador está offline; o APK foi construído e inspecionado, mas não executado nesta máquina.
- UI interna ainda é funcional/placeholder e precisa de direção visual final.
- Não há AAB, assinatura release, Google Play Billing ou SDK real de anúncios.
- Backend continua opcional e sem autenticação/ownership comercial; a campanha funciona offline.
- O teste de evento usa probabilidade em até 300 ciclos; RNG injetável continua recomendável.
- Save corrompido cai para jogo novo, ainda sem restauração de backup `.bak`.

Antes de continuar, rode `git status --short --branch` e `git log -10 --oneline`. A árvore deve estar limpa após o commit deste incremento.
