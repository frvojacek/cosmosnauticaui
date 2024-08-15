import { useState, useEffect, type FormEvent } from "react";
function App() {
    const [documents, setDocument] = useState<string[]>([]);

    const domain = import.meta.env.PROD ? "" : "https://localhost:7075";

    useEffect(() => {
        fetchDocuments()
    }, [])

    async function fetchDocuments() {
        const response = await fetch(domain + "/api/Document")
        const documents = await response.json()
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
            <td>{document}</td>
            <td>
                <a href={`${domain}/api/Document/${document}`}>
                    <button>Download</button>
                </a>
            </td>
        </tr>
    )

    return (
        <>
            <form onSubmit={handleSubmit}>
                <label htmlFor="file">File</label>
                <input id="file" name="file" type="file" />
                <button>Submit</button>
            </form>
            <table>
                <tbody>
                    {listDocuments}
                </tbody>
            </table>
        </>
    );
}

export default App;