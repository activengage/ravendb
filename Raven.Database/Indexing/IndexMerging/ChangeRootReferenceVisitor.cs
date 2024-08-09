using ICSharpCode.NRefactory.CSharp;

namespace Raven35.Database.Indexing.IndexMerging
{
    internal class ChangeRootReferenceVisitor : DepthFirstAstVisitor
    {
        private readonly string _fromIdentifier;

        public ChangeRootReferenceVisitor(string fromIdentifier)
        {
            _fromIdentifier = fromIdentifier;
        }

        public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
        {
            var identifierExpression = memberReferenceExpression.Target as IdentifierExpression;
            if (identifierExpression != null && identifierExpression.Identifier == _fromIdentifier)
            {
                memberReferenceExpression.Target = new IdentifierExpression("doc");
            }

            base.VisitMemberReferenceExpression(memberReferenceExpression);
        }
    }
}
