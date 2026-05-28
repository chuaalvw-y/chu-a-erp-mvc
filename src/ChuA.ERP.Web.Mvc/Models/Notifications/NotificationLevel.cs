// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Models.Notifications;

/// <summary>
/// Severity / styling hint for a <see cref="NotificationDto"/>. Maps cleanly to the
/// Bootstrap text-bg-* and alert-* utility classes.
/// </summary>
public enum NotificationLevel
{
    /// <summary>Default neutral notification.</summary>
    Info = 0,

    /// <summary>Successful operation (e.g. "Vendor saved").</summary>
    Success = 1,

    /// <summary>Caution / soft-failure.</summary>
    Warning = 2,

    /// <summary>Operation failed.</summary>
    Error = 3,
}
