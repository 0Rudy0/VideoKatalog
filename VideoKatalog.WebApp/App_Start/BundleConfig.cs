using System.Web;
using System.Web.Optimization;

namespace VideoKatalog.WebApp
{
	public class BundleConfig
	{
		// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
		public static void RegisterBundles(BundleCollection bundles)
		{
			#region Scripts

			bundles.Add(new ScriptBundle("~/bundles/_LayoutKatalog").Include(
				"~/Scripts/jquery-2.1.1.js",
				"~/Scripts/bootstrapJS/bootstrap-311.js",
				"~/Scripts/bootstrapJS/bootstrap-multiselect.js",
				//"~/Scripts/bootstrapJS/prettify.js",
				"~/Scripts/modernizr.custom.js",
				"~/Scripts/classie.js",
				"~/Scripts/sidebarEffects.js",
				"~/Scripts/jquery-ui.js",
				"~/Scripts/fancySelect.js",
				"~/Scripts/icheck.js",
				"~/Scripts/jquery.nouislider.js",
				"~/Scripts/jqwidgets/jqxcore.js",
				"~/Scripts/jqwidgets/jqxdata.js",
				"~/Scripts/jqwidgets/jqxbuttons.js",
				"~/Scripts/jqwidgets/jqxscrollbar.js",
				"~/Scripts/jqwidgets/jqxinput.js",
				"~/Scripts/jqwidgets/jqxwindow.js",
				"~/Scripts/jqwidgets/jqxpanel.js",
				"~/Scripts/jqwidgets/jqxtabs.js",
				"~/Scripts/jqwidgets/jqxslider.js",
				"~/Scripts/jqwidgets/jqxdatetimeinput.js",
				"~/Scripts/jqwidgets/jqxcalendar.js",
				"~/Scripts/jqwidgets/jqxtooltip.js",
				"~/Scripts/jqwidgets/jqxrangeselector.js",
				"~/Scripts/jqwidgets/jqxnumberinput.js",
				"~/Scripts/jqwidgets/jqxcombobox.js",
				"~/Scripts/jqwidgets/jqxcheckbox.js",
				"~/Scripts/jqwidgets/globalization/globalize.js",
				"~/Scripts/FileSaver.js",
                "~/Scripts/_LayoutKatalog.js"
				));

			bundles.Add(new ScriptBundle("~/bundles/_LayoutKatalog1").Include(
				"~/Scripts/jquery-2.1.1.js",
				"~/Scripts/bootstrapJS/bootstrap-311.js",
				"~/Scripts/bootstrapJS/bootstrap-multiselect.js",
				"~/Scripts/modernizr.custom.js",
				"~/Scripts/classie.js",
				"~/Scripts/sidebarEffects.js",
				"~/Scripts/jquery-ui.js",
				"~/Scripts/fancySelect.js",
				"~/Scripts/icheck.js",
				"~/Scripts/jquery.nouislider.js",
				"~/Scripts/jqwidgets/jqxcore.js",
				"~/Scripts/jqwidgets/jqxdata.js",
				"~/Scripts/jqwidgets/jqxbuttons.js",
				"~/Scripts/jqwidgets/jqxscrollbar.js",
				"~/Scripts/jqwidgets/jqxinput.js",
				"~/Scripts/jqwidgets/jqxwindow.js",
				"~/Scripts/jqwidgets/jqxpanel.js",
				"~/Scripts/jqwidgets/jqxtabs.js",
				"~/Scripts/jqwidgets/jqxslider.js",
				"~/Scripts/jqwidgets/jqxdatetimeinput.js",
				"~/Scripts/jqwidgets/jqxcalendar.js",
				"~/Scripts/jqwidgets/jqxtooltip.js",
				"~/Scripts/jqwidgets/jqxrangeselector.js",
				"~/Scripts/jqwidgets/jqxnumberinput.js",
				"~/Scripts/jqwidgets/jqxcombobox.js",
				"~/Scripts/jqwidgets/jqxcheckbox.js",
				"~/Scripts/jqwidgets/globalization/globalize.js"
				));


			bundles.Add(new ScriptBundle("~/bundles/MovieList2").Include(
				"~/Scripts/jqwidgets/jqxlistbox.js",
				"~/Scripts/movieList2.js"));

			bundles.Add(new ScriptBundle("~/bundles/SerieList2").Include(
				"~/Scripts/jqwidgets/jqxtree.js",
				"~/Scripts/serieList2.js"));

			#region noNeed

			//bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
			//			"~/Scripts/jquery-{version}.js"));					

			//bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
			//			"~/Scripts/jquery.unobtrusive*",
			//			"~/Scripts/jquery.validate*"));

			//bundles.Add(new ScriptBundle("~/bundles/_Layout").Include(
			//	"~/Scripts/jquery-1.11.1.js",
			//	"~/Scripts/bootstrap/bootstrap-3.0.3.min.js",
			//	"~/Scripts/bootstrap/bootstrap-multiselect.js",
			//	"~/Scripts/jqwidgets/jqxcore.js",
			//	"~/Scripts/jqwidgets/jqxwindow.js",
			//	"~/Scripts/slidingElement.js",
			//	"~/Scripts/modernizr-*",
			//	"~/Scripts/jquery.nouislider.min.js"));

			//bundles.Add(new ScriptBundle("~/bundles/_Layout").Include(
			//	"~/Scripts/jquery-1.10.2.min.js"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			//bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
			//			"~/Scripts/modernizr-*",
			//			"~/Scripts/jquery-1.10.2.min.js"));
			//bundles.Add(new ScriptBundle("~/bundles/modernizr2").Include(
			//			"~/Scripts/modernizr-*",
			//			"~/Scripts/jquery-1.10.2.min.js"));

			//bundles.Add(new ScriptBundle("~/bundles/MovieList").Include(
			//			"~/Scripts/changingIcons.js",
			//			"~/Scripts/movieList.js",
			//			"~/Scripts/fancySelect.js",
			//			"~/Scripts/jquery-ui.js"));


			#endregion

			#endregion

			#region Styles

			bundles.Add(new StyleBundle("~/Content/LayoutKatalog").Include(
				"~/Content/_LayoutKatalog.css",
				"~/Content/iCheck/skins/polaris/polaris.css",
				"~/Content/normalize.css",
				"~/Content/demo.css",
				"~/Content/icons.css",
				"~/Content/fancySelect.css",
				"~/Content/boostrapCss/bootstrap-311.css",
				"~/Content/boostrapCss/bootstrap-multiselect.css",
				//"~/Content/boostrapCss/prettify.css",
				"~/Content/component.css",
				"~/Content/jquery-ui.css",
				"~/Content/jqwidgetsStyle/jqx.base.css",
				"~/Content/jquery.nouislider.css",
				"~/Content/jqwidgetsStyle/jqx.metrodark.css"
				));

			//bundles.Add(new StyleBundle("~/Content/LayoutKatalog").Include(
			//	"~/Content/boostrapCss/bootstrap-311.css",
			//	"~/Content/boostrapCss/bootstrap-multiselect.css",
			//	"~/Content/boostrapCss/prettify.css"));

			bundles.Add(new StyleBundle("~/Content/MovieList2").Include(
				"~/Content/movieList2.css"));

			bundles.Add(new StyleBundle("~/Content/SerieList2").Include(
				"~/Content/serieList2.css"));

			#region noNeed

			//bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

			//bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
			//			"~/Content/themes/base/jquery.ui.core.css",
			//			"~/Content/themes/base/jquery.ui.resizable.css",
			//			"~/Content/themes/base/jquery.ui.selectable.css",
			//			"~/Content/themes/base/jquery.ui.accordion.css",
			//			"~/Content/themes/base/jquery.ui.autocomplete.css",
			//			"~/Content/themes/base/jquery.ui.button.css",
			//			"~/Content/themes/base/jquery.ui.dialog.css",
			//			"~/Content/themes/base/jquery.ui.slider.css",
			//			"~/Content/themes/base/jquery.ui.tabs.css",
			//			"~/Content/themes/base/jquery.ui.datepicker.css",
			//			"~/Content/themes/base/jquery.ui.progressbar.css",
			//			"~/Content/themes/base/jquery.ui.theme.css"));

			//bundles.Add(new StyleBundle("~/Content/Layout").Include(
			//	"~/Content/_Layout.css",
			//	"~/Content/bootstrap/bootstrap-3.0.3.min.css",
			//	"~/Content/bootstrap/bootstrap-multiselect.css",
			//	"~/Scripts/jqwidgets/styles/jqx.base.css",
			//	"~/Scripts/jqwidgets/styles/jqx.darkblue.css",
			//	"~/Content/fonts.css",
			//	"~/Content/slidingMenu.css"));

			//bundles.Add(new StyleBundle("~/Content/MovieList").Include(
			//	"~/Content/movieList.css",
			//	"~/Content/fancySelect.css",
			//	"~/Content/index.css",
			//	"~/Content/jquery-ui.css"));

			#endregion

			#endregion
		}
	}
}