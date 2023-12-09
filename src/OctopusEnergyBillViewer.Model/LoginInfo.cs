namespace OctopusEnergyBillViewer.Model;

/// <summary>
/// アカウント情報
/// </summary>
/// <param name="EmailAddress">Eメールアドレス</param>
/// <param name="Password">パスワード</param>
/// <param name="AccountNumber">お客様番号</param>
public record class AccountInfo(EmailAddress EmailAddress, Password Password, AccountNumber AccountNumber);

/// <summary>
/// お客様番号
/// </summary>
/// <param name="Value">値</param>
public record class AccountNumber(string Value);

/// <summary>
/// Eメールアドレス
/// </summary>
/// <param name="Value">値</param>
public record class EmailAddress(string Value);

/// <summary>
/// パスワード
/// </summary>
/// <param name="Value">値</param>
public record class Password(string Value);
