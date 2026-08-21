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

### 🔴 BLOQUEIO REGISTRADO: Unity Editor não pôde ser instalado de forma headless

- **Problema:** Não foi possível instalar o Unity Editor + módulo Android via linha de comando nesta sessão.
- **Motivo:** O Unity Hub instalado via winget é um pacote MSIX (executa em `C:\Program Files\WindowsApps\...`, sandbox sem alias de execução no PATH e sem shim de CLI). O instalador tradicional (`UnityHubSetup.exe`) via CDN direto retornou HTTP 404 (URL descontinuada) e a página oficial de download bloqueou fetch automatizado (HTTP 403). Mesmo que o binário do Hub fosse acessível via CLI (`-- --headless install ...`), a ativação de licença do Unity Personal normalmente exige login interativo (OAuth via navegador) ou uma chave serial — nenhum dos dois pode ser concluído por um agente headless.
- **Impacto:** Não é possível abrir o projeto no Editor, compilar via Unity, rodar Unity Test Runner ou gerar APK/AAB dentro desta sessão.
- **Solução necessária:** O usuário deve abrir o Unity Hub (já instalado) manualmente, fazer login/ativar a licença Personal, instalar uma versão LTS do Editor (recomendado: 2022 LTS ou mais recente disponível) com o módulo **Android Build Support** (inclui SDK/NDK/OpenJDK), e então abrir a pasta `StartupEmpire/` como projeto existente.
- **Mitigação adotada:** Todo o código de gameplay foi escrito already pronto para importar no Unity (estrutura `Assets/Game/**`, ScriptableObjects, MonoBehaviours). A lógica determinística (economia, idle, save, missões) foi implementada como C# puro (POCO, sem dependência de `UnityEngine`) e é validada por um projeto de testes **.NET real** (`Tests.NET/`) que compila e roda com `dotnet test` nesta máquina — evidência de execução real, não simulada. Quando o Editor for instalado, os mesmos scripts rodam dentro do Unity Test Runner sem alterações.

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
[ ] 21. Premium Currency
[ ] 22. Store
[ ] 23. Statistics
[ ] 24. UI final
[ ] 25. Audio
[ ] 26. Art polish
[ ] 27. Android optimization
[x] 28. Tests — 56 testes reais via `dotnet test` (ampliar cobertura conforme novos sistemas chegam; Unity Test Framework pendente do Editor)
[ ] 29. Balancing
[ ] 30. APK/AAB (depende do Editor instalado — ver bloqueio)
[ ] 31. Documentation
```

Este documento é atualizado conforme o `PROGRESS.md` avança. Detalhes de design em `GAME-DESIGN-DOCUMENT.md`, decisões técnicas em `ARCHITECTURE.md`.
