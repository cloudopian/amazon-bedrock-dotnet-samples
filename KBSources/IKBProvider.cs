using MyBedrockTest.Model;

namespace MyBedrockTest.KBSources
{
    internal interface IKBProvider
    {
        IEnumerable<KBArticle> GetKBArticles();
    }
}
