namespace ExpenseIncomeTracker

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating

[<JavaScript>]
module Client =

    type TransactionType =
        | Income
        | Expense

    type Transaction = {
        Description: string
        Amount: float
        TransactionType: TransactionType
    }

    type Model = {
        Transactions: ListModel<int, Transaction>
        Balance: Var<float>
    }

    let initModel() = {
        Transactions = ListModel.Create(fun t -> t.GetHashCode())
        Balance = Var.Create 0.0
    }

    let transactionForm model =
        let description = Var.Create ""
        let amount = Var.Create 0.0
        let transactionType = Var.Create Income

        let addTransaction () =
            let transaction = {
                Description = description.Value
                Amount = amount.Value
                TransactionType = transactionType.Value
            }
            model.Transactions.Add transaction
            match transaction.TransactionType with
            | Income -> model.Balance.Value <- model.Balance.Value + transaction.Amount
            | Expense -> model.Balance.Value <- model.Balance.Value - transaction.Amount

        div [] [
            input [attr.placeholder "Description"; bind.var description]
            input [attr.placeholder "Amount"; attr.typ "number"; bind.var amount]
            select [
                bind.var transactionType
                option [attr.value "Income"] [text "Income"]
                option [attr.value "Expense"] [text "Expense"]
            ]
            button [on.click (fun _ _ -> addTransaction ())] [text "Add Transaction"]
        ]

    let transactionList model =
        div [] [
            Doc.BindView (fun transactions ->
                div [] [
                    for t in transactions do
                        div [] [
                            text (sprintf "%s: %f (%A)" t.Description t.Amount t.TransactionType)
                        ]
                ]
            ) model.Transactions.View
        ]

    let balanceDisplay model =
        div [] [
            text "Balance: "
            Doc.BindView (fun balance -> textf "%f" balance) model.Balance.View
        ]

    [<SPAEntryPoint>]
    let Main () =
        let model = initModel()

        div [] [
            h1 [] [text "Income and Expenses Tracker"]
            transactionForm model
            transactionList model
            balanceDisplay model
        ]
        |> Doc.RunById "main"
