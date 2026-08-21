# PROJECT PLAN — Startup Empire

## 1. Auditoria do Ambiente (2026-08-20)

| Ferramenta | Status | Detalhe |
|---|---|---|
| Git | ✅ Disponível | 2.54.0.windows.1 |
| .NET SDK | ✅ Disponível | 8.0.422 e 10.0.301 instalados |
| JDK | ✅ Disponível | OpenJDK 21.0.11 LTS |
| Gradle | ⚠️ Não está no PATH | Existe cache em `~/.gradle` de projetos anteriores (Expo/RN); wrapper será usado quando necessário |
| Unity Hub | ✅ Instalado nesta sessão | Via `winget install Unity.UnityHub` (pacote MSIX 3.21.0.65535) |
| Unity Editor | ❌ Não instalado | Ver bloqueio abaixo |
| Android SDK | ❌ Não instalado | Normalmente instalado junto com o módulo Android do Unity Hub |

### ✅ BLOQUEIO RESOLVIDO: Unity Editor + módulo Android instalados via CLI headless

- **Problema original:** O Unity Hub instalado via winget é um pacote MSIX (sandbox sem CLI).
- **Como foi resolvido:** Baixei o instalador tradicional (`UnityHubSetup-x64.exe`, direto de `public-cdn.cloud.unity3d.com/hub/prod/` — a URL sem sufixo `-x64` estava descontinuada, mas essa variante existe) e reinstalei o Hub com ele. Isso deu acesso a `"Unity Hub.exe" -- --headless install/install-modules`, que **não exige login** para baixar/instalar o Editor e os módulos — só a *ativação de licença* (usar o Editor, não instalá-lo) exige.
- **Editor instalado:** 6000.0.82f1 (linha LTS) em `C:\Program Files\Unity\Hub\Editor\6000.0.82f1`.
- **Módulo Android:** completo (SDK + NDK + OpenJDK + build-tools, 5.4GB), confirmado em disco. Precisou de um contorno: o primeiro `install --module android` saiu antes de registrar os módulos no manifesto do Hub, fazendo `install-modules` falhar com "No modules found for this editor". Resolvido rodando manualmente o instalador NSIS do módulo (`UnitySetup-Android-Support-for-Editor-6000.0.82f1.exe`, que o Hub já tinha baixado em `%AppData%\UnityHub\downloads\`) e reassociando o Editor com `editors --add`, o que destravou o `install-modules` para NDK/SDK completarem.
- **Único passo que ainda depende do usuário:** login/ativação da licença Unity Personal na primeira vez que o Editor abrir um projeto (OAuth via navegador ou conta — não pode ser feito por um agente headless sem as credenciais).
- **Mitigação que já existia antes disso:** todo o código de gameplay já estava pronto para importar no Unity (estrutura `Assets/Game/**`), com a lógica determinística em C# puro validada por `Tests.NET/` via `dotnet test` — evidência real mesmo antes do Editor existir. Agora que o Editor está instalado, o próximo passo é abrir o projeto (após a ativação de licença) e rodar o Unity Test Runner/compilação real pela primeira vez.

## 2. Estado Inicial

- Diretório `C:\Users\Usuario` não continha nenhum projeto de jogo pré-existente.
- Projeto criado em `C:\Users\Usuario\StartupEmpire\`.
- Repositório Git inicializado do zero (`git init`), identidade global já configurada (RobersonCodes).

## 3. Ordem de Implementação (seguindo a missão, seção 36)

```
[x] 01. Auditoria do ambiente
[x] 02. Projeto Unity (estrutura de pastas + ProjectSettings mínimo)
[x] 03. Core/Game State
[x] 04. Save System
[x] 05. Time System
[x] 06. Economy
[x] 07. Primeiro capítulo (quarto → primeiro cliente)
[x] 08. Product Development (DevelopmentService: dev/test/fix/launch)
[x] 09. Bugs/Testing (bugs introduzidos por progresso, testados e corrigidos)
[x] 10. Customers/Sales (CustomerAcquisitionService: aquisição/conversão/churn)
[x] 11. Missions (sistema genérico + 4 missões do Cap. 1)
[x] 12. Upgrades (computador, internet, ferramentas, cursos — custo por nível + multiplicadores)
[x] 13. Idle (offline progress com teto de horas)
[x] 14. Research (trilhas de conhecimento + LearningService — árvore completa de nós fica para expansão futura)
[x] 15. Employees (10 cargos, contratação/demissão, folha de pagamento, produtividade por cargo)
[x] 16. Company Progression (gates Pessoa Física → Freelancer → Microempresa → Startup)
[x] 17. Events (sistema data-driven com escolhas e consequências: Servidor caiu, Bug crítico, Cliente importante)
[x] 18. Competitors (2 concorrentes simulados, crescimento simples, participação de mercado)
[x] 19. Investment (Angel/Seed/Series A/B/C com diluição real de equity; Ipo pendente de mecânica própria)
[x] 20. Achievements (Hello World, First Customer, MRR, Founder, Unicorn)
[x] 21. Premium Currency (Gems: saldo, ledger, grant/spend, pronto para Play Billing depois)
[x] 22. Store (boost de dev, boost de aquisição, aporte instantâneo, cosmético — sem mecânica predatória)
[x] 23. Statistics — StatisticsService agrega o GameState num snapshot legível (telas in-game ficam para a etapa de UI)
[~] 24. UI final (14/19 experiências: 13 telas navegáveis + modal de Events; faltam Splash, Main Menu, New Game, Continue e Development dedicada; navegação/eventos testados em PlayMode)
[x] 25. Audio (AudioManager com volume independente por categoria; sem clipes — nenhum áudio original disponível ainda)
[~] 26. Art polish (ícone original integrado; visual interno ainda é placeholder funcional)
[ ] 27. Android optimization
[x] 28. Tests — 90 `.NET` + 27 EditMode + 7 PlayMode (124 reais no cliente) + 22 no backend
[ ] 29. Balancing
[x] 30. APK/AAB — **APK de debug atual gerado**: `Builds/Android/StartupEmpire-debug.apk`, 45.648.468 bytes, SHA-256 `CDE073EE732B03957F68801E25C4723A0E4B21801822A23C972BE42B3374A6BF`, build IL2CPP+Gradle real (0 erros, 0 avisos) com ícone customizado. AAB ainda não gerado.
[ ] 31. Documentation
```

**Nota:** a lista acima segue a numeração 01–31 da seção 36 da missão, que não enumera explicitamente Ranking/Backend (seção 23 do corpo da missão) nem Referrals (seção 24 do corpo) — são itens à parte, implementados nesta sessão:

```
[x] Ranking/Backend (seção 23) — ASP.NET Core + PostgreSQL (Npgsql/EF Core) em backend/StartupEmpire.Api,
    validação server-side real, endpoints /api/ranking/submit|top|me. Ver backend/README.md.
[x] Referrals (seção 24) — código de indicação, vínculo inviter/invitee, recompensa, limite e
    prevenção de abuso, também em backend/StartupEmpire.Api. Cliente Unity com fallback offline-safe.
[x] IAdService (seção 22 do corpo — não confundir com "22. Store" da lista acima) — abstração
    de anúncios com NullAdService seguro por padrão, recompensa em Gems só quando o anúncio
    termina com sucesso de verdade.
```

Este documento é atualizado conforme o `PROGRESS.md` avança. Detalhes de design em `GAME-DESIGN-DOCUMENT.md`, decisões técnicas em `ARCHITECTURE.md`.
