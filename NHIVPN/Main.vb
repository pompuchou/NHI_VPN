Public Class Main
#Region "Declaration"
    '20191006 created
    '20191023 整理一下結構
    '1.0.0.6 20191023 表格, 下載專區
    Private Property Pageready As Boolean = False
#End Region

#Region "Buttons"
    Private Sub VPNToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VPNToolStripMenuItem.Click
        ' 20191006 created
#Region "Declaration"

#End Region

#Region "Prepare"
        ' 20191019 created
        ' 目的: 進入目標網頁
        ' 1. 入門網頁: https://medvpn.nhi.gov.tw/iwse0000/iwse0001s01.aspx
        ' 2. 登入頁面: https://medvpn.nhi.gov.tw/iwse0000/IWSE0020S01.aspx
        ' 3. 根部首頁: https://medvpn.nhi.gov.tw/iwpe0000/IWPE0000S22.aspx
        ' 4. 日期範圍: https://medvpn.nhi.gov.tw/ieae0000/IEAE0200S01.aspx
        ' 4.1 起始日: ContentPlaceHolder1_txtStartDate
        ' 4.2 結束日: ContentPlaceHolder1_txtEndDate
        ' 4.3 按按鈕: ContentPlaceHolder1_cmdQuery
        ' 5. 目標頁面: https://medvpn.nhi.gov.tw/ieae0000/IEAE0200S01.aspx
        ' 5.1 目標表單: ContentPlaceHolder1_gvDownLoad
        ' 5.2 轉換頁面: ContentPlaceHolder1_pgDownLoad
        Me.TabControl1.SelectedTab = Me.TabPage1
#End Region
        'MessageBox.Show(Me.WebBrowser1.Url.ToString)
        'Threading.Thread.Sleep(5000)
        'Me.WebBrowser1.Navigate("https://medvpn.nhi.gov.tw/iwse0000/IWSE0020S01.aspx")
        'WaitForPageLoad()
        '        MessageBox.Show(Me.WebBrowser1.Url.ToString)
        '        Threading.Thread.Sleep(5000)

        'Me.WebBrowser1.Document.GetElementById("cph_ctl00_btnLogin").InvokeMember("Click")
        'WaitForPageLoad()
        '' https://medvpn.nhi.gov.tw/iwpe0000/IWPE0000S22.aspx
        '        MessageBox.Show(Me.WebBrowser1.Url.ToString)
        '        Threading.Thread.Sleep(5000)

        Me.WebBrowser1.Navigate("https://medvpn.nhi.gov.tw/ieae0000/IEAE0200S01.aspx")

        'Dim iea As HtmlElement = Me.WebBrowser1.Document.GetElementById("IEA")
        'Dim a1 As HtmlElement = iea.Children(1).Children(1).Children(0)
        'a1.InvokeMember("Click")
        '' https://medvpn.nhi.gov.tw/ieae0000/IEAE0200S01.aspx
        'WaitForPageLoad()
        '' this is download page
        'MessageBox.Show(Me.WebBrowser1.Url.ToString)
        'Threading.Thread.Sleep(5000)
    End Sub

    Private Sub DownloadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DownloadToolStripMenuItem1.Click
        ' 20191020 created
        ' 存儲頁面
#Region "Declaration"
        Dim htmlgvList As HtmlElement
        Dim pg As HtmlElement
        Dim pg_N As Int16 = 1
        Dim header_want As String() = {"檔案名稱", "檔案說明", "下載備註", "提供下載日期 ", "檔案下載"}
        Dim header_order As New List(Of Int16)
        Dim order_n As Int16 = 0
        Dim current_time As Date = Now
#End Region

#Region "Prepare"
        Me.TabControl1.SelectedTab = Me.TabPage1
        ' 取得gvList
        htmlgvList = WebBrowser1.Document.GetElementById("ContentPlaceHolder1_gvDownLoad")
        If htmlgvList Is Nothing Then
            Exit Sub
        End If
        ' 找出要的順序
        order_n = 0
        For Each th As HtmlElement In htmlgvList.GetElementsByTagName("th")
            For i = 0 To header_want.Count - 1
                If th.InnerText.Replace(vbCrLf, "") = header_want(i) Then
                    header_order.Add(i)
                    '                            Exit For
                End If
            Next
            If header_order.Count = order_n Then
                header_order.Add(-1)
            End If
            order_n += 1
        Next
        ' 找到雲端藥歷有幾頁
        pg = htmlgvList.Document.GetElementById("ContentPlaceHolder1_pgDownLoad")
        If pg IsNot Nothing Then
            ' 有ContentPlaceHolder1_pg_gvList, 表示有多頁
            pg_N = pg.Children.Count - 5
        Else
            ' 沒有ContentPlaceHolder1_pg_gvList, 表示只有ㄧ頁
            pg_N = 1
        End If
#End Region

#Region "Write"
        ' 讀取第一頁
        Download_work(htmlgvList, header_order, current_time)

        ' 讀取第二至最後一頁
        ' FOR NEXT
        If pg_N > 1 Then
            For i = 1 To pg_N - 1
                ' 按按鈕
                ' 重點i+2, 要避免i+3的錯誤
                pg.Children.Item(i + 2).InvokeMember("Click")
                WaitForPageLoad()

                htmlgvList = WebBrowser1.Document.GetElementById("ContentPlaceHolder1_gvDownLoad")
                ' 找到雲端藥歷有幾頁
                pg = htmlgvList.Document.GetElementById("ContentPlaceHolder1_pgDownLoad")
                Download_work(htmlgvList, header_order, current_time)
            Next
        End If
#End Region

#Region "Ending"
        ' 匯入大表
        ' 這裡原本多了一次沒有try包覆的insert_p_cloudmed, 一但p_cloudmed有錯誤就沒辦法處理source
        ' 處理source
#End Region
    End Sub

    Private Sub SpecialToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SpecialToolStripMenuItem.Click
        ' 20191023 created
        ' 下載專區
#Region "Declaration"
        Dim htmlgvList As HtmlElement
        Dim pg As HtmlElement
        Dim pg_N As Int16 = 1
        Dim header_want As String() = {"檔案名稱", "資料說明檔案", "提供下載日期", "檔案下載"}
        Dim header_order As New List(Of Int16)
        Dim order_n As Int16 = 0
        Dim current_time As Date = Now
#End Region

#Region "Prepare"
        Me.TabControl1.SelectedTab = Me.TabPage1
        ' 取得gvList
        htmlgvList = WebBrowser1.Document.GetElementById("cph_rptDownload")
        If htmlgvList Is Nothing Then
            Exit Sub
        End If
        ' 找出要的順序
        order_n = 0
        For Each th As HtmlElement In htmlgvList.GetElementsByTagName("th")
            For i = 0 To header_want.Count - 1
                If th.InnerText.Replace(vbCrLf, "") = header_want(i) Then
                    header_order.Add(i)
                    '                            Exit For
                End If
            Next
            If header_order.Count = order_n Then
                header_order.Add(-1)
            End If
            order_n += 1
        Next
        ' 找到雲端藥歷有幾頁
        pg = htmlgvList.Document.GetElementById("cph_pgDownLoad")
        If pg IsNot Nothing Then
            ' 有ContentPlaceHolder1_pg_gvList, 表示有多頁
            pg_N = pg.Children.Count - 5
        Else
            ' 沒有ContentPlaceHolder1_pg_gvList, 表示只有ㄧ頁
            pg_N = 1
        End If
#End Region

#Region "Write"
        ' 讀取第一頁
        Special_work(htmlgvList, header_order, current_time)

        ' 讀取第二至最後一頁
        ' FOR NEXT
        If pg_N > 1 Then
            For i = 1 To pg_N - 1
                ' 按按鈕
                ' 重點i+2, 要避免i+3的錯誤
                pg.Children.Item(i + 2).InvokeMember("Click")
                WaitForPageLoad()

                htmlgvList = WebBrowser1.Document.GetElementById("cph_rptDownload")
                ' 找到雲端藥歷有幾頁
                pg = htmlgvList.Document.GetElementById("cph_pgDownLoad")
                Special_work(htmlgvList, header_order, current_time)
            Next
        End If
#End Region

#Region "Ending"
        ' 匯入大表
        ' 這裡原本多了一次沒有try包覆的insert_p_cloudmed, 一但p_cloudmed有錯誤就沒辦法處理source
        ' 處理source
#End Region

    End Sub
#End Region

    Private Sub Download_work(ByRef html As HtmlElement, ByRef header_order As List(Of Int16), ByRef current_time As DateTime)
        Dim dc As New NHIDataContext
        For Each tr As HtmlElement In html.GetElementsByTagName("tr")
            If tr.GetElementsByTagName("td").Count = 0 Then
                Continue For
            End If
            Dim order_n As Int16 = 0
            Dim s_f_name As String = ""
            Dim s_f_remark As String = ""
            Dim s_remark As String = ""
            Dim d_SDATE As DateTime = Now()
            Dim b_archive As Boolean
            For Each td As HtmlElement In tr.GetElementsByTagName("td")
                Select Case header_order(order_n)
                    Case 0  '檔案名稱
                        If td.InnerText IsNot Nothing Then
                            s_f_name = td.InnerText
                        End If
                    Case 1 '檔案說明
                        If td.InnerText IsNot Nothing Then
                            s_f_remark = td.InnerText
                        End If
                    Case 2 '下載備註
                        If td.InnerText IsNot Nothing Then
                            s_remark = td.InnerText
                        End If
                    Case 3 '提供下載日期
                        If td.InnerText IsNot Nothing Then
                            Dim temp_s As String() = td.InnerText.Split(vbCrLf)
                            Dim temp_d As String() = temp_s(0).Split("/")
                            d_SDATE = CStr(CInt(temp_d(0)) + 1911) + "/" + temp_d(1) + "/" + temp_d(2) + " " + temp_s(1)
                        End If
                    Case 4 '檔案下載
                        If td.Children.Count = 2 Then
                            b_archive = True
                        ElseIf td.Children.Count = 3 Then
                            b_archive = False
                        End If
                    Case Else
                End Select
                order_n += 1
            Next

            Dim q = From p In dc.tbl_download Where (p.f_name = s_f_name And p.SDATE = d_SDATE) Select p
            If q.Count = 0 Then
                Dim newNHI As New tbl_download With {.QDATE = current_time, .f_name = s_f_name, .f_remark = s_f_remark, .remark = s_remark, .SDATE = d_SDATE}
                '存檔
                If b_archive Then
                    newNHI.download = False
                Else
                    '20191023: 突然間firstchild不能用了,要改用children(0)
                    tr.Children(4).Children(0).InvokeMember("Click")
                    'Threading.Thread.Sleep(10000)
                    'Application.DoEvents()
                    'Threading.Thread.Sleep(500)
                    'SendKeys.Send(Keys.S)
                    Dim th_begin As New Threading.ThreadStart(AddressOf Work_todo)
                    Dim th As New Threading.Thread(th_begin) With {
                        .IsBackground = True,
                        .Name = "PressS"
                    }
                    th.Start()
                    WaitForPageLoad()
                    newNHI.download = True
                End If

                dc.tbl_download.InsertOnSubmit(newNHI)
                dc.SubmitChanges()
            End If
        Next
    End Sub

    Private Sub Special_work(ByRef html As HtmlElement, ByRef header_order As List(Of Int16), ByRef current_time As DateTime)
        Dim dc As New NHIDataContext
        For Each tr As HtmlElement In html.GetElementsByTagName("tr")
            If tr.GetElementsByTagName("td").Count = 0 Then
                Continue For
            End If
            Dim order_n As Int16 = 0
            Dim s_f_name As String = ""
            Dim s_f_remark As String = ""
            Dim d_SDATE As DateTime = Now()
            Dim b_archive As Boolean
            For Each td As HtmlElement In tr.GetElementsByTagName("td")
                Select Case header_order(order_n)
                    Case 0  '檔案名稱
                        If td.InnerText IsNot Nothing Then
                            s_f_name = td.InnerText
                        End If
                    Case 1 '資料說明檔案
                        If td.InnerText IsNot Nothing Then
                            s_f_remark = td.InnerText
                        End If
                    Case 2 '提供下載日期
                        If td.InnerText IsNot Nothing Then
                            Dim temp_s As String() = td.InnerText.Split(" ")
                            Dim temp_d As String() = temp_s(0).Split("/")
                            d_SDATE = CStr(CInt(temp_d(0)) + 1911) + "/" + temp_d(1) + "/" + temp_d(2) + " " + temp_s(1)
                        End If
                    Case 3 '檔案下載
                        If td.Children.Count = 1 Then
                            b_archive = True
                        ElseIf td.Children.Count = 2 Then
                            b_archive = False
                        End If
                    Case Else
                End Select
                order_n += 1
            Next

            Dim q = From p In dc.tbl_download Where (p.f_name = s_f_name And p.SDATE = d_SDATE) Select p
            If q.Count = 0 Then
                Dim newNHI As New tbl_download With {.QDATE = current_time, .f_name = s_f_name, .f_remark = s_f_remark, .SDATE = d_SDATE}
                '存檔
                If b_archive Then
                    newNHI.download = False
                Else
                    '20191023: 突然間firstchild不能用了,要改用children(0)
                    tr.Children(4).Children(0).InvokeMember("Click")
                    'Threading.Thread.Sleep(10000)
                    'Application.DoEvents()
                    'Threading.Thread.Sleep(500)
                    'SendKeys.Send(Keys.S)
                    Dim th_begin As New Threading.ThreadStart(AddressOf Work_todo)
                    Dim th As New Threading.Thread(th_begin) With {
                        .IsBackground = True,
                        .Name = "PressS"
                    }
                    th.Start()
                    WaitForPageLoad()
                    newNHI.download = True
                End If

                dc.tbl_download.InsertOnSubmit(newNHI)
                dc.SubmitChanges()
            End If
        Next
    End Sub

    Private Sub Work_todo()
        Threading.Thread.Sleep(4000)
        SendKeys.SendWait("s")
        Threading.Thread.Sleep(1000)
        SendKeys.SendWait("{Enter}")
    End Sub

#Region "Page Loading Functions"
    Private Sub WaitForPageLoad()
        AddHandler Me.WebBrowser1.DocumentCompleted, New WebBrowserDocumentCompletedEventHandler(AddressOf PageWaiter)
        ' We need a time out, say 10 sec, 10000 = 10 sec
        Dim ii As Int16 = 0
        While (Not Pageready) And (ii < 10000)
            Application.DoEvents()
            Threading.Thread.Sleep(1)
            ii += 1
        End While
        If ii >= 10000 Then
            RemoveHandler Me.WebBrowser1.DocumentCompleted, New WebBrowserDocumentCompletedEventHandler(AddressOf PageWaiter)
        End If
        Pageready = False
    End Sub

    Private Sub PageWaiter(ByVal sender As Object, ByVal e As WebBrowserDocumentCompletedEventArgs)
        If Me.WebBrowser1.ReadyState = WebBrowserReadyState.Complete Then
            Pageready = True
            RemoveHandler Me.WebBrowser1.DocumentCompleted, New WebBrowserDocumentCompletedEventHandler(AddressOf PageWaiter)
        End If
    End Sub

    Private Sub RefreshToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RefreshToolStripMenuItem.Click
        Dim dc As New NHIDataContext
        Me.DataGridView1.DataSource = dc.tbl_download
    End Sub
#End Region
End Class
