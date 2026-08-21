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
[ ] 08. Product Development (expandir além do MVP do Cap. 1)
[ ] 09. Bugs/Testing (sistema de bugs no produto)
[ ] 10. Customers/Sales (expandir)
[ ] 11. Missions
[ ] 12. Upgrades
[ ] 13. Idle
[ ] 14. Research
[ ] 15. Employees
[ ] 16. Company Progression
[ ] 17. Events
[ ] 18. Competitors
[ ] 19. Investment
[ ] 20. Achievements
[ ] 21. Premium Currency
[ ] 22. Store
[ ] 23. Statistics
[ ] 24. UI final
[ ] 25. Audio
[ ] 26. Art polish
[ ] 27. Android optimization
[ ] 28. Tests (ampliar cobertura)
[ ] 29. Balancing
[ ] 30. APK/AAB (depende do Editor instalado — ver bloqueio)
[ ] 31. Documentation
```

Este documento é atualizado conforme o `PROGRESS.md` avança. Detalhes de design em `GAME-DESIGN-DOCUMENT.md`, decisões técnicas em `ARCHITECTURE.md`.
