# Handoff para o Claude — atualizado em 2026-08-21

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
3. Ícone original Android integrado e APK reconstruído. Consulte `git log -7 --oneline` para os hashes mais recentes.
4. Incremento posterior: tempo como recurso real.
   - Dia atual e ciclos restantes persistidos no save schema V3.
   - Estudar/desenvolver/testar/corrigir consomem ciclo somente quando válidos.
   - `Encerrar Dia` executa um ciclo econômico e restaura quatro ciclos.
   - Office/Research exibem limite e motivo de falha.
5. UX Android portrait corrigida.
   - `SafeAreaFitter` para notch/barra de gestos.
   - Cinco destinos principais + grade `Mais` 3×3.
   - Voltar fecha menu/retorna ao Início.
   - Build trava portrait e `renderOutsideSafeArea=false`.

## Evidência verificada

- Domínio: 92/92 (`dotnet test Tests.NET/StartupEmpire.Domain.Tests.csproj`).
- Backend: 22/22 (`dotnet test backend/StartupEmpire.Api.Tests/StartupEmpire.Api.Tests.csproj`); dentro de sandbox o Windows Event Log causa falso negativo, portanto rode com permissão normal.
- Unity EditMode: 31/31.
- Unity PlayMode: 8/8.
- APK: `Builds/Android/StartupEmpire-debug.apk` (ignorado pelo Git), 45.610.490 bytes.
- SHA-256: `6D6373E0679613059F9BA445D0305BBED08E9B47C541C61BAE39FA131B013D6F`.
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

1. Criar Splash/Main Menu/New Game/Continue e tutorial contextual persistido.
2. Ligar emulador/dispositivo, instalar o APK atual e fazer smoke test + logcat antes de declarar runtime Android validado.
3. Definir unidade econômica do dia (salários hoje são cobrados por ciclo, apesar do rótulo mensal) e rebalancear com testes.
4. Adicionar scroll/ações bloqueadas com motivo nas telas de listas.

## Limitações que não devem ser esquecidas

- O teste do modal ainda usa sorteio probabilístico (300 ciclos); tornar o RNG injetável seria melhor.
- Save corrompido ainda cai em jogo novo sem backup `.bak` recuperável.
- Backend não tem autenticação/ownership comercial; `PlayerId` ainda é auto-declarado.
- Não existe AAB/release signing/CI; o APK atual é debug.
- A maior parte da UI interna ainda é placeholder; safe area global já foi corrigida.

Antes de continuar, rode `git status --short --branch` e leia `PROGRESS.md`. A working tree deve estar limpa após o commit final desta sessão.
