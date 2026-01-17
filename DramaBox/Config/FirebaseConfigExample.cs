//// Config/FirebaseConfigExample.cs
//// ======================================================
//// ARQUIVO DE EXEMPLO
//// ------------------------------------------------------
//// Copie este arquivo para:
////   FirebaseConfig.cs
//// e preencha com suas credenciais reais do Firebase.
////
//// ⚠️ NÃO versionar FirebaseConfig.cs no GitHub
//// ======================================================

//namespace DramaBox.Config;

//public static class FirebaseConfig
//{
//    // ===== Firebase (Web App Config) =====
//    // Encontrado em: Firebase Console > Configurações do Projeto > Geral
//    public static string ApiKey = "SUA_API_KEY_AQUI";
//    public static string ProjectId = "SEU_PROJECT_ID_AQUI";

//    // Realtime Database URL (SEM barra final)
//    // Ex: https://seu-projeto-default-rtdb.firebaseio.com
//    public static string DatabaseUrl = "https://SEU_PROJETO.firebaseio.com";

//    // Storage bucket
//    // Ex: seu-projeto.appspot.com OU seu-projeto.firebasestorage.app
//    public static string StorageBucket = "SEU_BUCKET_AQUI";

//    // (Opcional) Mantidos para referência/uso futuro
//    public static string MessagingSenderId = "SEU_SENDER_ID";
//    public static string AppId = "SEU_APP_ID";

//    // ===== Endpoints REST Firebase Auth =====
//    public static string SignUpUrl =>
//        $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={ApiKey}";

//    public static string SignInUrl =>
//        $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ApiKey}";

//    public static string GetAccountUrl =>
//        $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={ApiKey}";

//    // ===== Helpers =====
//    public static string NormalizeDbUrl(string url)
//        => (url ?? "").Trim().TrimEnd('/');

//    public static string DbBase => NormalizeDbUrl(DatabaseUrl);

//    // Endpoint REST do Firebase Storage (upload/download metadata)
//    public static string StorageBase =>
//        $"https://firebasestorage.googleapis.com/v0/b/{StorageBucket}/o";

//    // Realtime Database root (sem barra no final)
//    // Ex: https://seu-projeto-default-rtdb.firebaseio.com
//    public static string RealtimeBaseUrl { get; set; } =
//        "https://SEU_PROJETO.firebaseio.com";

//    // Token opcional para Realtime Database
//    // Normalmente vazio quando usando regras públicas ou Auth via Firebase
//    public static string RealtimeAuthToken { get; set; } = "";
//}
