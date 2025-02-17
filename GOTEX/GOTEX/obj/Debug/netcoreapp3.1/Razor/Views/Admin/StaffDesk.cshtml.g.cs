#pragma checksum "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\Admin\StaffDesk.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "90dd542f3a4e5438932064d0418562050b75e9e6"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Admin_StaffDesk), @"mvc.1.0.view", @"/Views/Admin/StaffDesk.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\_ViewImports.cshtml"
using GOTEX;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\_ViewImports.cshtml"
using GOTEX.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\_ViewImports.cshtml"
using GOTEX.ViewModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\_ViewImports.cshtml"
using GOTEX.Core.BusinessObjects;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\_ViewImports.cshtml"
using GOTEX.Core.Repositories;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\Admin\StaffDesk.cshtml"
using System.Linq;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\Admin\StaffDesk.cshtml"
using Microsoft.EntityFrameworkCore;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"90dd542f3a4e5438932064d0418562050b75e9e6", @"/Views/Admin/StaffDesk.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"553fa4994824a0e1c271dcc0934bac47a5a1b66a", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Admin_StaffDesk : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<List<ApplicationUser>>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/Scripts/jquery-3.3.1.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
            WriteLiteral("\r\n");
#nullable restore
#line 8 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\Admin\StaffDesk.cshtml"
  
    ViewBag.Title = "Applications on STaff Desk";
    int count = 0;
    var roles = _role.Roles.ToList();

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<center>
    <div class="" card"" style=""margin-top: 2em; padding: 1em;"">
        <h2 class=""card"" style=""font-size: x-large;"">
            <b>Application(s) on Staff Desk</b>
        </h2>
        <table class=""table table-striped dataTable"">
            <thead>
            <tr>
                <th>#</th>
                <th>Full Name</th>
                <th>Email</th>
                <th>User Role</th>
                <th>Desk Count</th>
                <th></th>
            </tr>
            </thead>
            <tbody>
");
#nullable restore
#line 31 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\Admin\StaffDesk.cshtml"
             foreach (var user in Model)
            {
                if (!_userManager.IsInRoleAsync(user, "Company").Result)
                {
                   
                    count++;
                    var apps = GetDeskCount(user.Email);

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr>\r\n                        <td>");
#nullable restore
#line 39 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\Admin\StaffDesk.cshtml"
                       Write(count);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td>");
#nullable restore
#line 40 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\Admin\StaffDesk.cshtml"
                       Write(user.LastName);

#line default
#line hidden
#nullable disable
            WriteLiteral(" ");
#nullable restore
#line 40 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\Admin\StaffDesk.cshtml"
                                      Write(user.FirstName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td>");
#nullable restore
#line 41 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\Admin\StaffDesk.cshtml"
                       Write(user.Email);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td>\r\n");
#nullable restore
#line 43 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\Admin\StaffDesk.cshtml"
                                  
                                    var role = GetUserRole(user.Email);
                                

#line default
#line hidden
#nullable disable
            WriteLiteral("                                ");
#nullable restore
#line 46 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\Admin\StaffDesk.cshtml"
                           Write(role.DisplayName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n                        <td class=\"text-center\">");
#nullable restore
#line 48 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\Admin\StaffDesk.cshtml"
                                           Write(GetDeskCount(user.Email));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td>\r\n                                <a href=\"#appModal\" data-toggle=\"modal\" class=\"btn btn-warning btn-xs btnViewStaffDesk\" data-email=\"");
#nullable restore
#line 50 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\Admin\StaffDesk.cshtml"
                                                                                                                               Write(user.Id);

#line default
#line hidden
#nullable disable
            WriteLiteral("\">View Desk</a>\r\n                        </td>\r\n                    </tr> \r\n");
#nullable restore
#line 53 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\Admin\StaffDesk.cshtml"
                }
            }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"            </tbody>
        </table>
    </div>
</center>
<div class=""modal"" role=""dialog"" tabindex=""-1"" id=""appModal"">
    <div class=""modal-dialog modal-dialog-centered modal-lg"" role=""document"">
        <div class=""modal-content"">
            <div class=""text-center""><h3 class=""modal-title""></h3></div>
            <div class=""modal-body"" id=""modalBody"">
            </div>
            <div class=""modal-footer"">
                <button type=""button"" class=""btn btn-secondary"" data-dismiss=""modal"">Close</button>
            </div>
        </div>
    </div>
</div>
");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "90dd542f3a4e5438932064d0418562050b75e9e69681", async() => {
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
            WriteLiteral(@"
<script type=""text/javascript"">
    $(document).ready(function () {
        $('.dataTable').dataTable({ dom: 'Blfrtip'});

        $(document).on('click', '.btnViewStaffDesk', function (e) {
            e.preventDefault();
            $('.modal-title').html('Applications on Staff desk');
            var user = $(this).data('email');
            console.log(""befoe..."", user);
            $.get('/Admin/StaffApps', { id: user }, function (data) {
                console.log(""data..."", data);
                $('.modal-body').html(data);
            });
        });
    });
</script>
");
        }
        #pragma warning restore 1998
#nullable restore
#line 71 "C:\Users\BrandOneTech\source\repos\GOTEX\COGATEX\GOTEX\GOTEX\Views\Admin\StaffDesk.cshtml"
           

    private ApplicationRole GetUserRole(string email)
    {
        var user = _userManager.Users.Include(ur => ur.UserRoles).ThenInclude(r => r.Role).FirstOrDefault(e => e.Email.Equals(email));
        return user.UserRoles.FirstOrDefault().Role;
    }

    private int GetDeskCount(string email) => _application.GetAll().Count(x => x.LastAssignedUserId == email);

    private List<ApplicationUser> GetStaffRoleList(string email)
    {
        var role = GetUserRole(email);
        return _userManager.GetUsersInRoleAsync(role.Name).Result.ToList();
    }


#line default
#line hidden
#nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public RoleManager<ApplicationRole> _role { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public UserManager<ApplicationUser> _userManager { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public IApplication<Application> _application { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<List<ApplicationUser>> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
