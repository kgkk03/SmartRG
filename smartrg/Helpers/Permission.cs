using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Xamarin.Essentials.Permissions;

namespace smartrg.Helpers
{
    public class Permission
    {
        public static async Task<bool> CheckPemiison<TPermission>() where TPermission : BasePermission, new()
        {
            try
            {
                // Check ก่อน ว่าอนุญาตไหม
                PermissionStatus hasPermission = await Permissions.CheckStatusAsync<TPermission>();

                if (hasPermission == PermissionStatus.Granted)
                {
                    return await Task.FromResult(true);
                }
                else // ไม่อนุญาตให้ไปขอ ReqPermision<TPermission>();
                {
                    bool resPermission = await ReqPermision<TPermission>();
                    if (resPermission)
                    {
                        return await Task.FromResult(true);
                    }
                    else
                    {
                        return await Task.FromResult(false);
                    }
                }
            }
            catch (Exception ex)
            {
                var a = ex.Message;
                return await Task.FromResult(false);
            }
        }

        public static async Task<bool> ReqPermision<TPermission>() where TPermission : BasePermission, new()
        {
            try
            {
                PermissionStatus result = await Permissions.RequestAsync<TPermission>();
                if (result == PermissionStatus.Granted)
                {
                    return await Task.FromResult(true);
                }
                else
                {
                    return await Task.FromResult(false);
                }
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }

    }
}
