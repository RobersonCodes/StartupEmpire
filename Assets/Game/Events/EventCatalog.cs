using System;
using System.Collections.Generic;
using StartupEmpire.Core;
using StartupEmpire.Economy;
using StartupEmpire.Products;

namespace StartupEmpire.Events
{
    /// Eventos do Capítulo 1 (exemplos da seção 14 da missão), cada um com
    /// consequências reais e mutuamente exclusivas por escolha.
    public static class EventCatalog
    {
        public static List<GameEventDefinition> CreateChapter1Catalog()
        {
            return new List<GameEventDefinition>
            {
                new GameEventDefinition
                {
                    Id = "server_down",
                    Title = "Servidor caiu",
                    Description = "Um dos seus produtos lançados ficou fora do ar.",
                    TriggerCondition = AnyProductLaunched,
                    Choices = new List<EventChoice>
                    {
                        new EventChoice
                        {
                            Id = "restart",
                            Label = "Reiniciar rapidamente",
                            Apply = (state, economy) => ForEachLaunchedProduct(state,
                                p => p.Stability = Math.Max(0, p.Stability - 0.05))
                        },
                        new EventChoice
                        {
                            Id = "investigate",
                            Label = "Investigar a causa",
                            Apply = (state, economy) => ForEachLaunchedProduct(state,
                                p => p.Stability = Math.Min(1, p.Stability + 0.10))
                        },
                        new EventChoice
                        {
                            Id = "invest_infra",
                            Label = "Contratar infraestrutura melhor",
                            Apply = (state, economy) =>
                            {
                                economy.TrySpend(state.Economy, 200, LedgerCategory.Infrastructure, "infra_upgrade_event");
                                ForEachLaunchedProduct(state, p => p.Stability = Math.Min(1, p.Stability + 0.20));
                            }
                        }
                    }
                },

                new GameEventDefinition
                {
                    Id = "critical_bug",
                    Title = "Bug crítico encontrado em produção",
                    Description = "Clientes reportaram um bug grave no seu produto lançado.",
                    TriggerCondition = AnyProductLaunched,
                    Choices = new List<EventChoice>
                    {
                        new EventChoice
                        {
                            Id = "fix_now",
                            Label = "Corrigir imediatamente",
                            Apply = (state, economy) => ForEachLaunchedProduct(state, p =>
                            {
                                p.BugCount = p.BugCount / 2;
                                p.KnownBugCount = Math.Min(p.KnownBugCount, p.BugCount);
                            })
                        },
                        new EventChoice
                        {
                            Id = "ignore",
                            Label = "Ignorar por enquanto",
                            Apply = (state, economy) => ForEachLaunchedProduct(state,
                                p => p.Reputation = Math.Max(0, p.Reputation - 0.05))
                        },
                        new EventChoice
                        {
                            Id = "pay_overtime",
                            Label = "Pagar hora extra para corrigir",
                            Apply = (state, economy) =>
                            {
                                if (economy.TrySpend(state.Economy, 150, LedgerCategory.Salary, "overtime_bugfix_event"))
                                {
                                    ForEachLaunchedProduct(state, p =>
                                    {
                                        p.BugCount = 0;
                                        p.KnownBugCount = 0;
                                        p.Stability = Math.Min(1, p.Stability + 0.10);
                                    });
                                }
                            }
                        }
                    }
                },

                new GameEventDefinition
                {
                    Id = "big_client_offer",
                    Title = "Um cliente importante ofereceu um contrato maior",
                    Description = "Uma empresa maior quer fechar um contrato com você.",
                    TriggerCondition = AnyPayingCustomer,
                    Choices = new List<EventChoice>
                    {
                        new EventChoice
                        {
                            Id = "accept",
                            Label = "Aceitar o contrato",
                            Apply = (state, economy) => economy.Earn(state.Economy, 300, "big_client_contract")
                        },
                        new EventChoice
                        {
                            Id = "negotiate",
                            Label = "Negociar condições melhores",
                            Apply = (state, economy) =>
                            {
                                economy.Earn(state.Economy, 200, "big_client_contract_negotiated");
                                ForEachLaunchedProduct(state, p => p.Reputation = Math.Min(1, p.Reputation + 0.05));
                            }
                        },
                        new EventChoice
                        {
                            Id = "decline",
                            Label = "Recusar",
                            Apply = (state, economy) => ForEachLaunchedProduct(state,
                                p => p.Reputation = Math.Min(1, p.Reputation + 0.02))
                        }
                    }
                }
            };
        }

        private static void ForEachLaunchedProduct(GameState state, Action<ProductState> action)
        {
            foreach (var product in state.Products)
            {
                if (product.Stage == ProductStage.Launched || product.Stage == ProductStage.Maintenance)
                {
                    action(product);
                }
            }
        }

        private static bool AnyProductLaunched(GameState state)
        {
            foreach (var product in state.Products)
            {
                if (product.Stage == ProductStage.Launched || product.Stage == ProductStage.Maintenance) return true;
            }
            return false;
        }

        private static bool AnyPayingCustomer(GameState state)
        {
            foreach (var product in state.Products)
            {
                if (product.PayingCustomers > 0) return true;
            }
            return false;
        }
    }
}
