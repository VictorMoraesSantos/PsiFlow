using Auth.Domain.Entities;

namespace Auth.Infrastructure.Persistence.Seeds
{
    public static class PermissionGroupSeed
    {
        public static IReadOnlyCollection<PermissionGroup> GetDefaultGroups()
        {
            var groups = new List<PermissionGroup>
            {
                CreateCrudGroup("dashboard", "Dashboard", "Permissões do dashboard"),
                CreateCrudGroup("restaurants", "Restaurantes", "Permissões de restaurante"),
                CreateCrudGroup("units", "Unidades", "Permissões de unidades"),
                CreateCrudGroup("users", "Usuários", "Permissões de usuários"),
                CreateCrudGroup("roles", "Perfis", "Permissões de perfis e permissões"),
                CreateCrudGroup("settings", "Configurações", "Permissões de configurações operacionais"),
                CreateCrudGroup("menu", "Cardápio", "Permissões de cardápio"),
                CreateCrudGroup("tables", "Mesas", "Permissões de mesas"),
                CreateCrudGroup("tabs", "Comandas", "Permissões de comandas"),
                CreateCrudGroup("orders", "Pedidos", "Permissões de pedidos"),
                CreateCrudGroup("production", "Produção", "Permissões de cozinha e bar"),
                CreateCrudGroup("payments", "Pagamentos", "Permissões de pagamentos"),
                CreateCrudGroup("cash_register", "Caixa", "Permissões de caixa"),
                CreateCrudGroup("reports", "Relatórios", "Permissões de relatórios"),
                CreateCrudGroup("audit", "Auditoria", "Permissões de auditoria"),
                CreateCrudGroup("notifications", "Notificações", "Permissões de notificações")
            };
            return groups;
        }

        private static PermissionGroup CreateCrudGroup(string key, string name, string description)
        {
            var group = new PermissionGroup(key, name, description);
            group.AddDefaultCrudPermissions();
            return group;
        }
    }
}