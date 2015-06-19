using AQILib;
using AQILib.Gui;
using PathQueryLib;
using PathwaysLib.PathQuery;
using PathwaysLib.ServerObjects;
using PathwaysLib.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PathwaysLib.AQI.Path
{
    /// <summary>
    /// Renderer for the path-based queries (including both path and neighborhood)
    /// </summary>
    public class PathRenderer : AQILib.IQueryRenderer
    {
        private IAQIUtil _util;
        private string _nodeTitle;
        private string _edgeTitle;
        private string _nodeLinkKey; //me or pw
        private string _edgeLinkKey; //pc or me

        private PathRenderer()
        { }

        public PathRenderer(IAQIUtil util, string nodeTitle, string edgeTitle, string nodeLinkKey, string edgeLinkKey)
        {
            _util = util;
            _nodeTitle = nodeTitle;
            _edgeTitle = edgeTitle;
            _nodeLinkKey = nodeLinkKey;
            _edgeLinkKey = edgeLinkKey;
        }

        public IGuiComponent Render(QNode rootNode, AQILib.IQueryResults results, IGuiData data, out IGuiData dataOut)
        {
            PathQueryResults pathResults = (PathQueryResults) results;

            // Do we have an error? Return it.
            if(pathResults.IsErrorResult)
            {
                dataOut = null;
                return new GuiPath(pathResults.ErrorString);
            }

            // Otherwise, render either the neighborhood or the path
            if(pathResults.PathResults is QueryNeighborhoodResults)
                return RenderNeighborhood(rootNode, (QueryNeighborhoodResults) pathResults.PathResults, data, out dataOut);
            else if(pathResults.PathResults is QueryPathResults)
                return RenderPath(rootNode, (QueryPathResults) pathResults.PathResults, data, out dataOut);

            // We *should* never get here
            dataOut = null;
            return new GuiPath("Internal Typing Error.");
        }

        /// <summary>
        /// Render a set of neighborhood query results
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="neighborhoodResults"></param>
        /// <param name="data"></param>
        /// <param name="dataOut"></param>
        /// <returns></returns>
        public IGuiComponent RenderNeighborhood(QNode rootNode, QueryNeighborhoodResults neighborhoodResults, IGuiData data, out IGuiData dataOut)
        {
            // Give a message if there are no results
            if(neighborhoodResults.Nodes.Count <= 0)
            {
                dataOut = null;
                return new GuiPath(String.Format("The query did not return any results.{0}", neighborhoodResults.TimeoutReached ? String.Format(" (The query timeout limit of {0} seconds was reached.)", neighborhoodResults.Parameters.TimeoutLimit) : ""));
            }

            #region Molecule/Process Lists
            //SortedDictionary<string, List<string>> moleculeListing = new SortedDictionary<string, List<string>>();
            //SortedDictionary<string, List<string>> processListing = new SortedDictionary<string, List<string>>();
            //foreach(INode node in neighborhoodResults.Nodes)
            //{
            //    if(node.Type == NodeType.Node)
            //    {
            //        if(!moleculeListing.ContainsKey(node.Label))
            //            moleculeListing.Add(node.Label, new List<string>());
            //        moleculeListing[node.Label].Add(String.Format(_util.AQIForwardLink, node.Id, "me", node.Label));
            //    }
            //    else if(node.Type == NodeType.Edge)
            //    {
            //        if(!processListing.ContainsKey(node.Label))
            //            processListing.Add(node.Label, new List<string>());
            //        processListing[node.Label].Add(String.Format(_util.AQIForwardLink, node.Id, "pc", node.Label));
            //    }
            //    else
            //    {
            //        dataOut = null;
            //        return new GuiPath("Internal error: Invalid node type.");
            //    }
            //}
            #endregion

            /***** Grid Panel, this always stays *****/
            Panel gridPanel = new Panel();
            {
                //gridPanel.Style.Add(HtmlTextWriterStyle.Width, "100%");
                gridPanel.CssClass = "itemdisplay";

                Panel limitPanel = neighborhoodResults.LimitReached ? CreateLiteral(String.Format("NOTE: This query reached the maximum allowed limit of {0} results. A listing of the partial results are being shown here.", neighborhoodResults.Parameters.MaxResultLimit)) : null;

                Panel timeoutPanel = neighborhoodResults.TimeoutReached ? CreateLiteral(String.Format("NOTE: This query reach the timeout limit of {0} seconds. A listing of the partial results are being shown here.", neighborhoodResults.Parameters.TimeoutLimit)) : null;

                #region Molecule/Process Lists
                //DataGrid moleculeGrid = new DataGrid();
                //{
                //    DataTable dtMolecule = new DataTable();
                //    dtMolecule.Columns.Add("Molecules", typeof(string));
                //    foreach(List<string> moleculeTexts in moleculeListing.Values)
                //        foreach(string moleculeText in moleculeTexts)
                //            dtMolecule.Rows.Add(moleculeText);

                //    moleculeGrid.DataSource = dtMolecule;
                //    moleculeGrid.DataBind();
                //    moleculeGrid.CssClass = "datagridbase";
                //    moleculeGrid.CellPadding = 4;
                //    moleculeGrid.HeaderStyle.CssClass = "datagridheader";
                //    moleculeGrid.ItemStyle.CssClass = "datagriditem";

                //    moleculeGrid.Style.Add(HtmlTextWriterStyle.Width, "49%");
                //    moleculeGrid.Style.Add("float", "left");
                //    moleculeGrid.Style.Add("clear", "none");
                //}

                //DataGrid processGrid = new DataGrid();
                //{
                //    DataTable dtProcess = new DataTable();
                //    dtProcess.Columns.Add("Processes", typeof(string));
                //    foreach(List<string> processTexts in processListing.Values)
                //        foreach(string processText in processTexts)
                //            dtProcess.Rows.Add(processText);

                //    processGrid.DataSource = dtProcess;
                //    processGrid.DataBind();
                //    processGrid.CssClass = "datagridbase";
                //    processGrid.CellPadding = 4;
                //    processGrid.HeaderStyle.CssClass = "datagridheader";
                //    processGrid.ItemStyle.CssClass = "datagriditem";

                //    processGrid.Style.Add(HtmlTextWriterStyle.Width, "49%");
                //    processGrid.Style.Add("float", "right");
                //    processGrid.Style.Add("clear", "none");
                //}
                #endregion

                #region Grid - All Steps
                //DataGrid gridAll = new DataGrid();
                //{
                //    DataTable dt = new DataTable();
                //    dt.Columns.Add("Step", typeof(int));

                //    if(neighborhoodResults.Parameters.StartNode.Type == NodeType.Node)
                //    {
                //        dt.Columns.Add("Molecules", typeof(string));
                //        dt.Columns.Add("Processes", typeof(string));
                //    }
                //    else
                //    {
                //        dt.Columns.Add("Processes", typeof(string));
                //        dt.Columns.Add("Molecules", typeof(string));
                //    }

                //    int minPathLength;
                //    int maxPathLength;
                //    neighborhoodResults.PathLengths(out minPathLength, out maxPathLength);

                //    for(int i = minPathLength; i <= maxPathLength; i++)
                //    {
                //        SortedDictionary<string, List<string>> moleculeStrings = new SortedDictionary<string, List<string>>();
                //        foreach(INode node in neighborhoodResults.GetNodes(NodeType.Node, i))
                //        {
                //            if(!moleculeStrings.ContainsKey(node.Label))
                //                moleculeStrings.Add(node.Label, new List<string>());
                //            moleculeStrings[node.Label].Add(String.Format(_util.AQIForwardLink, node.Id, "me", node.Label));
                //        }

                //        List<string> molecules = new List<string>();
                //        foreach(List<string> moleculeStringList in moleculeStrings.Values)
                //            foreach(string moleculeString in moleculeStringList)
                //                molecules.Add(moleculeString);

                //        SortedDictionary<string, List<string>> processStrings = new SortedDictionary<string, List<string>>();
                //        foreach(INode node in neighborhoodResults.GetNodes(NodeType.Edge, i))
                //        {
                //            if(!processStrings.ContainsKey(node.Label))
                //                processStrings.Add(node.Label, new List<string>());
                //            processStrings[node.Label].Add(String.Format(_util.AQIForwardLink, node.Id, "pc", node.Label));
                //        }

                //        List<string> processes = new List<string>();
                //        foreach(List<string> processStringList in processStrings.Values)
                //            foreach(string processString in processStringList)
                //                processes.Add(processString);

                //        if(neighborhoodResults.Parameters.StartNode.Type == NodeType.Node)
                //            dt.Rows.Add(i, String.Join("<br />", molecules.ToArray()), String.Join("<br />", processes.ToArray()));
                //        else
                //            dt.Rows.Add(i, String.Join("<br />", processes.ToArray()), String.Join("<br />", molecules.ToArray()));
                //    }

                //    gridAll.DataSource = dt;
                //    gridAll.DataBind();
                //    gridAll.CssClass = "datagridbase";
                //    gridAll.CellPadding = 4;
                //    gridAll.HeaderStyle.CssClass = "datagridheader";
                //    gridAll.ItemStyle.CssClass = "datagriditem";
                //}
                #endregion

                // Construct the tabular grid
                #region Grid - Shortest Steps
                DataGrid gridShortest = new DataGrid();
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Step", typeof(int));

                    if(neighborhoodResults.StartNode.Type == NodeType.Node)
                    {
                        dt.Columns.Add(_nodeTitle, typeof(string));
                        dt.Columns.Add(_edgeTitle, typeof(string));
                    }
                    else
                    {
                        dt.Columns.Add(_edgeTitle, typeof(string));
                        dt.Columns.Add(_nodeTitle, typeof(string));
                    }

                    int minDistance;
                    int maxDistance;
                    neighborhoodResults.Distances(out minDistance, out maxDistance);

                    for(int i = minDistance; i <= maxDistance; i++)
                    {
                        SortedDictionary<string, List<string>> nodeStrings = new SortedDictionary<string, List<string>>();
                        foreach(INode node in neighborhoodResults.GetNodes(NodeType.Node, i))
                        {
                            if (!nodeStrings.ContainsKey(node.Label))
                            {
                                nodeStrings.Add(node.Label, new List<string>());
                                //BE: removing items with duplicate names here
                                nodeStrings[node.Label].Add(String.Format(_util.AQIForwardLink, node.Id, _nodeLinkKey, node.Label));
                            }
                        }

                        List<string> nodes = new List<string>();
                        foreach(List<string> nodeStringList in nodeStrings.Values)
                            foreach(string nodeString in nodeStringList)
                                nodes.Add(nodeString);

                        SortedDictionary<string, List<string>> edgeStrings = new SortedDictionary<string, List<string>>();
                        foreach(INode node in neighborhoodResults.GetNodes(NodeType.Edge, i))
                        {
                            if (!edgeStrings.ContainsKey(node.Label))
                            {
                                edgeStrings.Add(node.Label, new List<string>());
                                //BE: removing items with duplicate names here
                                edgeStrings[node.Label].Add(String.Format(_util.AQIForwardLink, node.Id, _edgeLinkKey, node.Label));
                            }
                        }

                        List<string> edges = new List<string>();
                        foreach(List<string> edgeStringList in edgeStrings.Values)
                            foreach(string edgeString in edgeStringList)
                                edges.Add(edgeString);

                        if(neighborhoodResults.StartNode.Type == NodeType.Node)
                            dt.Rows.Add(i, String.Join("<br />", nodes.ToArray()), String.Join("<br />", edges.ToArray()));
                        else
                            dt.Rows.Add(i, String.Join("<br />", edges.ToArray()), String.Join("<br />", nodes.ToArray()));
                    }

                    gridShortest.DataSource = dt;
                    gridShortest.DataBind();
                    gridShortest.CssClass = "datagridbase";
                    gridShortest.CellPadding = 4;
                    gridShortest.HeaderStyle.CssClass = "datagridheader";
                    gridShortest.ItemStyle.CssClass = "datagriditem";
                }
                #endregion

                if(limitPanel != null)
                    gridPanel.Controls.Add(limitPanel);

                if(timeoutPanel != null)
                    gridPanel.Controls.Add(timeoutPanel);

                //gridPanel.Controls.Add(moleculeGrid);
                //gridPanel.Controls.Add(processGrid);
                //gridPanel.Controls.Add(PathwaysLib.WebControls.ControlBuilder.BuildLiteral(@"<br clear=""all"" />"));

                //gridPanel.Controls.Add(gridAll);

                gridPanel.Controls.Add(gridShortest);

                // Hidden Graph Text added below if needed
            }

            // Hide the graph if the number of nodes exceeds 200 or if the graph has no molecules or no processes
            if(neighborhoodResults.DisplayGraph)
            {
                if(_nodeTitle == "Molecules")
                    dataOut = new GuiPathQueryDataOut(new GraphSourceNeighborhoodMetabolicNW(neighborhoodResults));
                else if(_nodeTitle == "Pathways")
                    dataOut = new GuiPathQueryDataOut(new GraphSourceNeighborhoodPWLinks(neighborhoodResults));
                else
                    dataOut = new GuiPathQueryDataOut(null);

            }
            else
            {
                dataOut = new GuiPathQueryDataOut(null);
                gridPanel.Controls.Add(CreateLiteral(neighborhoodResults.HiddenGraphText));
            }

            return new GuiPath(gridPanel);
        }

        /// <summary>
        /// Render a set of path query results
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="pathResults"></param>
        /// <param name="data"></param>
        /// <param name="dataOut"></param>
        /// <returns></returns>
        public IGuiComponent RenderPath(QNode rootNode, QueryPathResults pathResults, IGuiData data, out IGuiData dataOut)
        {
            if(pathResults.Paths.Count <= 0)
            {
                dataOut = null;
                return new GuiPath(String.Format("The query did not return any results.{0}", pathResults.TimeoutReached ? String.Format(" (The query timeout limit of {0} seconds was reached.)", pathResults.Parameters.TimeoutLimit) : ""));
            }

            Panel gridPanel = new Panel();
            {
                //gridPanel.Style.Add(HtmlTextWriterStyle.Width, "100%");

                Panel limitPanel = pathResults.LimitReached ? CreateLiteral(String.Format("NOTE: This query reached the maximum allowed limit of {0} results. A listing of the partial results are being shown here.", pathResults.Parameters.MaxResultLimit)) : null;

                Panel timeoutPanel = pathResults.TimeoutReached ? CreateLiteral(String.Format("NOTE: This query reach the timeout limit of {0} seconds. No results are being shown here.", pathResults.Parameters.TimeoutLimit)) : null;

                DataGrid grid = new DataGrid();
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Length", typeof(int));
                    //dt.Columns.Add("Path", typeof(string));

                    if(pathResults.StartNode.Type == NodeType.Node)
                    {
                        dt.Columns.Add(_nodeTitle, typeof(string));
                        dt.Columns.Add(_edgeTitle, typeof(string));
                    }
                    else
                    {
                        dt.Columns.Add(_edgeTitle, typeof(string));
                        dt.Columns.Add(_nodeTitle, typeof(string));
                    }

                    dt.Columns.Add("PathStart", typeof(bool));

                    int minLength;
                    int maxLength;
                    pathResults.Lengths(out minLength, out maxLength);

                    //for(int i = minLength; i <= maxLength; i++)
                    //{
                    //    List<List<INode>> paths = pathResults.GetPaths(i);

                    //    List<string> pathStrings = new List<string>();
                    //    foreach(List<INode> path in paths)
                    //    {
                    //        List<string> formattedNodeStrings = new List<string>();
                    //        foreach(INode node in path)
                    //            formattedNodeStrings.Add(String.Format(_util.AQIForwardLink, node.Id, node.Type == NodeType.Node ? _nodeLinkKey : _edgeLinkKey, node.Label));
                    //        pathStrings.Add(String.Join(" -> ", formattedNodeStrings.ToArray()));
                    //    }

                    //    dt.Rows.Add(i, String.Join("<br /><br />", pathStrings.ToArray()));
                    //}

                    for(int i = minLength; i <= maxLength; i++)
                    {
                        List<List<INode>> paths = pathResults.GetPaths(i);

                        foreach(List<INode> path in paths)
                        {
                            int ptr = 0;
                            while(ptr < path.Count)
                            {
                                if(ptr + 2 > path.Count)
                                    dt.Rows.Add(i,
                                                String.Format(_util.AQIForwardLink, path[ptr].Id, path[ptr].Type == NodeType.Node ? _nodeLinkKey : _edgeLinkKey, path[ptr].Label),
                                                "",
                                                ptr == 0);
                                else
                                    dt.Rows.Add(i,
                                                String.Format(_util.AQIForwardLink, path[ptr].Id, path[ptr].Type == NodeType.Node ? _nodeLinkKey : _edgeLinkKey, path[ptr].Label),
                                                String.Format(_util.AQIForwardLink, path[ptr + 1].Id, path[ptr + 1].Type == NodeType.Node ? _nodeLinkKey : _edgeLinkKey, path[ptr + 1].Label),
                                                ptr == 0);

                                ptr += 2;
                            }
                        }
                    }

                    grid.DataSource = dt;
                    grid.ItemDataBound += new DataGridItemEventHandler(grid_ItemDataBound);
                    grid.DataBind();
                    grid.CssClass = "datagridbase";
                    grid.CellPadding = 4;
                    grid.HeaderStyle.CssClass = "datagridheader";
                    grid.ItemStyle.CssClass = "datagriditem";
                }

                if(limitPanel != null)
                    gridPanel.Controls.Add(limitPanel);

                if(timeoutPanel != null)
                    gridPanel.Controls.Add(timeoutPanel);

                gridPanel.Controls.Add(grid);

                // Hidden Graph Text added below if needed
            }

            // Hide the graph if the number of nodes exceeds 200 or if the graph has no molecules or no processes
            if(pathResults.DisplayGraph)
            {
                if(_nodeTitle == "Molecules")
                    dataOut = new GuiPathQueryDataOut(new GraphSourcePathMetabolicNW(pathResults));
                else if(_nodeTitle == "Pathways")
                    dataOut = new GuiPathQueryDataOut(new GraphSourcePathPWLinks(pathResults));
                else
                    dataOut = new GuiPathQueryDataOut(null);
            }
            else
            {
                dataOut = new GuiPathQueryDataOut(null);
                gridPanel.Controls.Add(CreateLiteral(pathResults.HiddenGraphText));
            }

            return new GuiPath(gridPanel);
        }

        /// <summary>
        /// A helper function that creates the effect of a single path having a single spanning row for the length, even though there are several rows for the node data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void grid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            e.Item.Cells.RemoveAt(3);

            DataTable dt = ((DataTable) ((DataGrid) e.Item.Parent.Parent).DataSource);

            if(!(0 <= e.Item.ItemIndex && e.Item.ItemIndex <= dt.Rows.Count - 1))
                return;

            DataTableReader dtr = dt.CreateDataReader();
            dtr.Read();
            for(int i = 0; i < e.Item.ItemIndex; i++)
                dtr.Read();

            bool pathStart = (bool) dtr.GetValue(3);

            if(!pathStart)
            {
                e.Item.Cells.RemoveAt(0);
            }
            else
            {
                int rowSpanHeight = 1;
                while(dtr.Read() && !((bool) dtr.GetValue(3)))
                    rowSpanHeight += 1;
                e.Item.Cells[0].RowSpan = rowSpanHeight;
            }
            
            return;
        }

        private static Panel CreateLiteral(string message)
        {
            Panel res = new Panel();
            res.CssClass = "whitebg";
            Literal msg = new Literal();
            msg.Text = message;
            res.Controls.Add(msg);
            return res;
        }
    }
}