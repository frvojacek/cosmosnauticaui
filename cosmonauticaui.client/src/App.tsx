import { useState, useEffect, type FormEvent } from "react";

import './App.css';

interface Document {
    id: number
    name: string
    file: string
    places: string[]
    counterParties: string[]
    products: string[]
}

function App() {
    const [documents, setDocument] = useState<Document[]>([]);

    const domain = import.meta.env.PROD ? "" : "https://localhost:7075";

    useEffect(() => {
        fetchDocuments()
    }, [])

    async function fetchDocuments() {
        const response = await fetch(domain + "/api/Document")
        const documents = await response.json() as Document[]
        setDocument(documents)
    }
    
    async function handleSubmit(event: FormEvent) {
        event.preventDefault();
        const target = event.target as HTMLFormElement;
        await fetch(`${domain}/api/Document`, {
            method: "POST",
            body: new FormData(target)
        });
        fetchDocuments()
    }
    
    const listDocuments = documents.map((document, index) =>
        <tr key={index}>
            <td>{document.name}</td>
            <td>{document.places}</td>
            <td>{document.counterParties}</td>
            <td>{document.products}</td>
            <td>
                <a href={`${domain}/api/Document/${document}`}>
                    <button>Download</button>
                </a>
            </td>
        </tr>
    )

    return (
        <>
            <form id="document-create" onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="Name">Name</label>
                    <input id="name" name="name" />
                </div>
                <div>
                    <label htmlFor="file">File</label>
                    <input id="file" name="file" type="file" />
                </div>
                <div>
                    <label htmlFor="places">Places</label>
                    <input id="places" name="places" />
                </div>
                <div>
                    <label htmlFor="counterParties">Counter Parties</label>
                    <input id="counterParties" name="counterParties" />
                </div>
                <div>
                    <label htmlFor="products">Products</label>
                    <input id="products" name="products" />
                </div>
                <button>Submit</button>
            </form>
            <table>
                <thead>
                    <td>Document Name</td>
                    <td>Places</td>
                    <td>Counter parties</td>
                    <td>Products</td>
                </thead>
                <tbody>
                    {listDocuments}
                </tbody>
            </table>
        </>
    );
}

export default App;