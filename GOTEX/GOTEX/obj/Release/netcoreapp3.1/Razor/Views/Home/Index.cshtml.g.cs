#pragma checksum "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\Home\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1ac37f2fd0d7fabf8f302a5a0db68d80ceef3887"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Index), @"mvc.1.0.view", @"/Views/Home/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\_ViewImports.cshtml"
using GOTEX;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\_ViewImports.cshtml"
using GOTEX.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\_ViewImports.cshtml"
using GOTEX.ViewModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\_ViewImports.cshtml"
using GOTEX.Core.BusinessObjects;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\_ViewImports.cshtml"
using GOTEX.Core.Repositories;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1ac37f2fd0d7fabf8f302a5a0db68d80ceef3887", @"/Views/Home/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"553fa4994824a0e1c271dcc0934bac47a5a1b66a", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/Images/icon-lg1.png"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/Images/icon-lg2.png"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/Images/icon-lg3.png"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\Home\Index.cshtml"
  
    ViewData["Title"] = "Home Page";
    
    string VerifyPermitLink = _appConfiguration.GetElpsUrl() + "/Permit/VerifyPermit?appId=" + _appConfiguration.GetAppId();
    string ResendEmailActivationLink = _appConfiguration.GetElpsUrl() + "/Account/ResendEmailActivation?appId=" + _appConfiguration.GetAppId();

    string AccountLoginLink = _appConfiguration.GetElpsUrl() + "/Account/Login?appId=" + _appConfiguration.GetAppId();
    string AccountRegisterLink = _appConfiguration.GetElpsUrl() + "/Account/Register?appId=" + _appConfiguration.GetAppId();
    string ForgetPasswordLink = _appConfiguration.GetElpsUrl() + "/Account/ForgotPassword?appId=" + _appConfiguration.GetAppId();

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div class=\"col-md-12\">\r\n    <div class=\"col-md-5 form-signin extForm\">\r\n");
#nullable restore
#line 15 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\Home\Index.cshtml"
         if (ViewData["Message"] != null)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"            <div class=""col-md-12"">
                <div class=""alert alert-danger alert-dismissible center-block"" role=""alert"" id=""welcomealert"" align=""center"">
                    <button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""Close""><span aria-hidden=""true"">&times;</span></button>
                    <i class=""fa fa-exclamation-triangle fa-2x"" aria-hidden=""true""></i>
                    <strong>");
#nullable restore
#line 21 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\Home\Index.cshtml"
                       Write(ViewData["Message"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>\r\n                </div>\r\n            </div>\r\n");
#nullable restore
#line 24 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\Home\Index.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"        <div class=""padd"">
            <h1 class=""form-signin-heading text-center"">DPR</h1>
            <h4 class=""txtcenter"">GAS TERMINAL EXPORT PORTAL (GATEX)</h4>

            <a class="" btn btn-md btn-info btn-block textUpper buttonRadius1 round"" href=""/validate/"">Click here to Verify Permit</a>
            <br/>
            <a");
            BeginWriteAttribute("href", " href=\"", 1742, "\"", 1766, 1);
#nullable restore
#line 31 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\Home\Index.cshtml"
WriteAttributeValue("", 1749, AccountLoginLink, 1749, 17, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\" btn btn-md btn-success btn-block textUpper buttonRadius1 round\">LOGIN</a>\r\n            <br/>\r\n            <a");
            BeginWriteAttribute("href", " href=\"", 1884, "\"", 1911, 1);
#nullable restore
#line 33 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\Home\Index.cshtml"
WriteAttributeValue("", 1891, AccountRegisterLink, 1891, 20, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\" btn btn-md btn-success btn-block textUpper buttonRadius1 round\">REGISTER</a>\r\n            <br/>\r\n            <p class=\"txtcenter\">\r\n                <a");
            BeginWriteAttribute("href", " href=\"", 2071, "\"", 2097, 1);
#nullable restore
#line 36 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\Home\Index.cshtml"
WriteAttributeValue("", 2078, ForgetPasswordLink, 2078, 19, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"btn-link\">Forgot Password</a>\r\n            </p>\r\n\r\n            <br/>\r\n            <strong>Payment Processor:</strong>\r\n            <br/>\r\n            <div");
            BeginWriteAttribute("class", " class=\"", 2260, "\"", 2268, 0);
            EndWriteAttribute();
            WriteLiteral(">\r\n                <img");
            BeginWriteAttribute("src", " src=\"", 2292, "\"", 2337, 1);
#nullable restore
#line 43 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\Home\Index.cshtml"
WriteAttributeValue("", 2298, Url.Content("/Images/remita-logo.png"), 2298, 39, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" alt=\"Remita\" title=\"REMITA\"");
            BeginWriteAttribute("style", " style=\"", 2366, "\"", 2374, 0);
            EndWriteAttribute();
            WriteLiteral(@"/>
            </div>
            <br/><br/>
            <strong>Payment Options:</strong>
            <br/>
            <ul class=""paymentOptions"">
                <li><i class=""fa fa-check""></i> &nbsp; Cash Deposit</li>
                <li><i class=""fa fa-check""></i> &nbsp; Bank Draft</li>
                <li><i class=""fa fa-check""></i> &nbsp; Card Payment: MasterCard, Verve Card</li>
                <li><i class=""fa fa-check""></i> &nbsp; Quick Teller</li>
            </ul>
        </div>
    </div>

    <div class=""col-md-7 adv bg1"">
        <div class=""col-md-12"">
            <h2 class=""txtcenter"">Getting Started</h2>
        </div>
        <div class=""howto"">
            <div class=""col-md-5"">
                <div class=""icon"">
                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "1ac37f2fd0d7fabf8f302a5a0db68d80ceef388710336", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                </div>\r\n            </div>\r\n            <div class=\"col-md-7\">\r\n                <h3>User Guide</h3>\r\n                <a href=\"#\" target=\"_blank\" class=\"white\">Online Application Guide</a><br />\r\n                <a");
            BeginWriteAttribute("href", " href=\"", 3425, "\"", 3449, 1);
#nullable restore
#line 70 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\Home\Index.cshtml"
WriteAttributeValue("", 3432, AccountLoginLink, 3432, 17, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" class=""white"">Resend Activation Email</a><br />
                <a href=""/validate/"" class=""white"">Verify a Permit</a><br />
                <a href=""#"" class=""white"">FAQ</a><br />
            </div>
        </div>
        <br class=""clear"" /><br />
        <div class=""howto"">
            <div class=""col-md-5"">
                <div class=""icon"">
                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "1ac37f2fd0d7fabf8f302a5a0db68d80ceef388712365", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                </div>\r\n            </div>\r\n            <div class=\"col-md-7\">\r\n                <h3>\r\n                    <a");
            BeginWriteAttribute("href", " href=\"", 3989, "\"", 4016, 1);
#nullable restore
#line 84 "D:\Documents\Projects\COGATEX\GOTEX\GOTEX\Views\Home\Index.cshtml"
WriteAttributeValue("", 3996, AccountRegisterLink, 3996, 20, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" class=""white"">Company Account Creation</a>
                </h3>
                <p>
                    Create account to use resources on this portal. Note that Account creation
                    is not required to validate a permit.
                </p>
            </div>
        </div>
        <br class=""clear"" /><br />
        <div class=""howto"">
            <div class=""col-md-5"">
                <div class=""icon"">
                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "1ac37f2fd0d7fabf8f302a5a0db68d80ceef388714357", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
                </div>
            </div>
            <div class=""col-md-7"">
                <h3><a href=""/"" class=""white"">Manage Approvals</a></h3>
                <p>Login to apply for a new Application Form, renew or manage your approvals</p>
            </div>
        </div>
        <div class=""clear""><br /><br /></div>

    </div>
</div>

");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public IAppConfiguration<Configuration> _appConfiguration { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
