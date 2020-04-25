// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "It is safe to suppress a warning from this rule if the code library will not be localized or if the string is not exposed to the end user or a developer using the code library.")]
[assembly: SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "It is safe to suppress a warning from this rule when the library or application is intended for a limited local audience and will therefore not be localized.")]
[assembly: SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "You can suppress this warning if you know that the consumer is not a graphical user interface (GUI) app or if the consumer does not have a SynchronizationContext.")]
