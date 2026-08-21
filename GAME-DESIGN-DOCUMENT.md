# Startup Empire — Game Design Document

## Pitch

Tycoon + Idle + Strategy + Business Simulator com fio narrativo de carreira. O jogador começa como uma pessoa sozinha, num quarto, aprendendo a programar, e evolui até comandar uma empresa de tecnologia global. Não é apenas cliques em botões: cada decisão envolve recursos limitados, tempo, risco e consequência.

Plataforma: Android (portrait, otimizado para toque com uma mão / polegar). Arquitetura pensada para portfólio hoje, publicação comercial amanhã.

## Pilares de Design

1. **Progressão legível** — o jogador sempre entende por que está ganhando ou perdendo.
2. **Decisões com trade-off real** — dinheiro, tempo e qualidade competem entre si; não há escolha "de graça".
3. **Idle respeitoso** — progresso offline existe, mas nunca substitui o prazer de jogar ativamente.
4. **Data-driven** — números de balanceamento vivem em ScriptableObjects/JSON, nunca hardcoded.
5. **Honestidade sistêmica** — bugs, churn e crises são parte do ciclo, não punição arbitrária.

## Game Loop Macro

```
APRENDER → DESENVOLVER → TESTAR → ENCONTRAR BUGS → CORRIGIR → LANÇAR
→ CONQUISTAR CLIENTES → GERAR RECEITA → REINVESTIR → MELHORAR PRODUTO
→ CONTRATAR → ESCALAR → CRIAR NOVOS PRODUTOS
```

Cada volta do loop é impulsionada por **Tempo** (ciclos de trabalho / "sprints") e **Dinheiro** (caixa disponível), os dois recursos centrais do jogo.

## Progressão de Empresa

```
Pessoa física → Freelancer → Microempresa → Startup → Empresa → Scale-up → Empresa Global → IPO
```

Cada estágio desbloqueia mecânicas novas (contratação, pesquisa, investidores, concorrência, etc.) — ver `ARCHITECTURE.md` seção Progression para os gates exatos.

## Capítulo 1 — "O Quarto" (MVP jogável)

**Estado inicial:** computador básico, R$ 500 em caixa, 0 funcionários, 0 clientes, 0 reputação, conhecimento = 0 em todas as trilhas.

**Arco do capítulo:**
1. Tutorial contextual (balões curtos, nunca parede de texto) ensina a abrir o painel de Desenvolvimento.
2. Jogador aloca ciclos de trabalho para **Aprender** (ganha XP/Knowledge em uma trilha, ex: Fundamentos) ou **Desenvolver** (avança um Produto).
3. Ao atingir progresso suficiente, o produto tem sua primeira versão pronta para **Testar**.
4. Testar revela **Bugs** (quantidade influenciada por Qualidade do código, que depende de Knowledge e do computador).
5. Jogador escolhe **Corrigir** bugs (custa ciclos) ou **Lançar** mesmo assim (risco: reputação inicial menor, churn maior).
6. Produto lançado começa a **Conquistar Clientes** (função de Reputação, Qualidade, Marketing=0 no cap.1).
7. Clientes pagantes geram **Receita** por ciclo.
8. Receita permite **Reinvestir**: melhorar o computador (upgrade), comprar internet melhor, etc.
9. Ao atingir o primeiro cliente pagante e a primeira receita, desbloqueia a transição para **Freelancer**.

**Fim do capítulo:** primeira venda registrada, conquista "First Customer" desbloqueada, tela de transição para o próximo estágio.

## Sistema de Produtos

Atributos por produto (ver `ProductData`/`ProductState` em código): nome, categoria (Site, App, SaaS, Jogo, API, Ferramenta), qualidade, estabilidade, bugCount, performance, segurança, popularidade, usuários, clientesPagantes, preço, receita, custos, reputação, estágio (Planejamento → Desenvolvimento → Teste → Lançado → Manutenção → Descontinuado).

Fórmulas centrais (documentadas também em `EconomyConfig`):

- `QualityGain = BaseDevRate * (1 + KnowledgeBonus) * (1 + ToolBonus)`
- `BugsIntroduced = DevProgressThisTick * BugRatePerProgress * (1 - QualityFactor)`
- `BugsFound = TestingEffort * TestEfficiency`
- `ReputationDelta = (Quality - BugSeverityPenalty - ChurnPenalty) * ReputationSensitivity`

## Economia

Dinheiro, Receita, Despesas, Lucro, Fluxo de Caixa, MRR (receita recorrente), Salários, Infraestrutura, Marketing, Equipamentos, Custo Operacional, Valuation. Todos os multiplicadores/custos-base vivem em `EconomyConfig` (ScriptableObject) — nenhum número mágico no código de sistema.

`Valuation = (MRR * 12) * MultiplicadorSetor * FatorCrescimento - Dívidas`

## Sistema Idle

Ao pausar/fechar: grava `lastSaveTimestampUtc`. Ao retomar: calcula `elapsed = min(now - lastSaveTimestampUtc, MaxOfflineDuration)`, simula produção/receita/custos em batch (não tick-a-tick) e mostra um resumo ("Enquanto você esteve fora..."). Usa `DateTime.UtcNow` (não horário local, que é manipulável) e limita ganhos ofensivos por um teto configurável — mitigação razoável, não DRM invasivo.

## Clientes

Aquisição = f(Reputação, Qualidade, Marketing, Preço). Churn = f(Estabilidade, Suporte, Concorrência). Satisfação afeta ambos. Fórmulas documentadas em `CustomerConfig`.

## Funcionários

Backend/Frontend/Mobile Developer, Designer, QA, DevOps, PM, Marketing, Sales, Support. Atributos: salário, experiência, produtividade, qualidade, velocidade, especialização, satisfação. Simulação intencionalmente simples no MVP (multiplicadores lineares); complexidade cresce conforme o estágio da empresa evolui.

## Pesquisa

Árvore extensível por dados (`ResearchNodeData`): Fundamentos → Web → Banco de Dados → Backend → Frontend → Mobile → Cloud → DevOps → IA → Automação → Segurança. Cada nó desbloqueia bônus (ex: -X% bugs, +Y% dev speed) e pode ser pré-requisito de outros.

## Eventos

Sistema data-driven (`GameEventData`) com escolhas e consequências (ex.: "Servidor caiu" → reiniciar / investigar / investir em infra). Nunca puramente decorativo — cada escolha altera estado real do jogo.

## Investimento

Bootstrapping → Anjo → Seed → Series A/B/C → IPO. Cada rodada dá caixa em troca de equity (diluição simplificada, uma barra "Equity do Fundador" visível na UI). Sem dinheiro grátis.

## Missões e Conquistas

Sistema genérico orientado a dados, desacoplado da UI (`MissionDefinition` + `MissionService` observam o `GameState` via eventos, não via referência direta a Views).

## Moeda Premium (Gems) e Loja

Abstração completa de saldo/gasto/itens, sem conexão a pagamento real nesta fase. Preparado para Google Play Billing futuramente. Loja vende boosts e cosméticos — sem mecânicas predatórias (sem pay-to-win que quebre o loop offline).

## Anúncios

Interface `IAdService` com implementação mock/segura durante o desenvolvimento; lógica de jogo nunca referencia SDK de anúncio diretamente.

## Direção de Arte

Visual 2D estilizado, paleta própria (tons de "software escuro" — azul petróleo, ciano de destaque, acentos em laranja para alertas), tipografia geométrica. Nenhum asset de Game Dev Tycoon, Software Inc., Startup Company ou Idle Office Tycoon é referenciado ou copiado. Placeholders proceduais (formas simples, cor sólida) são aceitáveis durante o desenvolvimento e serão substituídos antes de uma versão "concluída".

## Telas Previstas

Splash, Main Menu, New Game, Continue, Office (hub central), Character, Products, Development, Employees, Research, Missions, Upgrades, Finances, Statistics, Company, Events, Achievements, Store, Settings.
